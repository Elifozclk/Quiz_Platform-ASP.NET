using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Net.Http;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QP_WEBPROJECT.vs2.Pages.Admin
{
    public partial class DeleteRequests : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserType"] == null || Session["UserType"].ToString() != "admin")
            {
                Response.Redirect("/Pages/User/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadRequests();
            }
        }

        private void LoadRequests()
        {
            try
            {
                string sql = @"
                    SELECT
                        dr.Id,
                        dr.UserId,
                        u.UserName,
                        u.Email,
                        COALESCE(dr.Reason, 'Sebep belirtilmedi') AS Reason,
                        COALESCE(dr.Status, 'pending') AS Status,
                        dr.RequestedAt
                    FROM DeleteRequests dr
                    INNER JOIN Users u ON u.Id = dr.UserId
                    ORDER BY
                        CASE COALESCE(dr.Status, 'pending')
                            WHEN 'pending' THEN 1
                            WHEN 'approved' THEN 2
                            WHEN 'rejected' THEN 3
                        END,
                        dr.RequestedAt DESC";

                DataTable dt = DbHelper.Query(sql);
                gvRequests.DataSource = dt;
                gvRequests.DataBind();

                if (lblStats != null && dt != null)
                {
                    int pending = dt.Select("Status = 'pending'").Length;
                    int approved = dt.Select("Status = 'approved'").Length;
                    int rejected = dt.Select("Status = 'rejected'").Length;

                    lblStats.Text = $"Toplam: {dt.Rows.Count} | Bekleyen: {pending} | Onaylanan: {approved} | Reddedilen: {rejected}";
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Sunucu hatası: " + ex.Message, "danger");
            }
        }

        // ✅ GridView butonları buraya düşecek
        protected void gvRequests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "approve" || e.CommandName == "reject")
            {
                int requestId = Convert.ToInt32(e.CommandArgument);
                bool success = false;

                if (e.CommandName == "approve")
                {
                    success = CallApiDelete($"/api/admin/deleterequest/{requestId}");
                }
                else if (e.CommandName == "reject")
                {
                    success = CallApiPut($"/api/admin/deleterequest/{requestId}/reject");
                }

                if (success)
                {
                    LoadRequests();
                    ShowMessage("Kullanıcı başarıyla silindi.", "success");
                }
                else
                {
                    // API hata mesajını buraya yansıtmak hata tespiti için önemlidir
                    ShowMessage("Silme işlemi başarısız oldu. Veritabanı kısıtlamalarını kontrol edin.", "danger");
                }
            }
        }

        private bool CallApiDelete(string apiUrl)
        {
            using (var client = new HttpClient())
            {
                // Port numarasının doğruluğundan emin olun
                client.BaseAddress = new Uri(Request.Url.GetLeftPart(UriPartial.Authority));

                // ÖNEMLİ: DELETE isteği gönderiyoruz
                var response = client.DeleteAsync(apiUrl).Result;

                return response.IsSuccessStatusCode;
            }
        }

        private bool CallApiPut(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Request.Url.GetLeftPart(UriPartial.Authority));
                    var resp = client.PutAsync(url, new StringContent("{}", System.Text.Encoding.UTF8, "application/json")).Result;
                    return resp.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        protected void gvRequests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            string status = DataBinder.Eval(e.Row.DataItem, "Status")?.ToString() ?? "pending";

            if (status == "pending") e.Row.CssClass = "table-warning";
            else if (status == "approved") e.Row.CssClass = "table-success";
            else if (status == "rejected") e.Row.CssClass = "table-danger";
        }

        protected string GetStatusBadge(string status)
        {
            if (string.IsNullOrEmpty(status)) status = "pending";

            switch (status.ToLower())
            {
                case "pending":
                    return "<span class='badge bg-warning text-dark'>Bekliyor</span>";
                case "approved":
                    return "<span class='badge bg-success'>Onaylandı</span>";
                case "rejected":
                    return "<span class='badge bg-danger'>Reddedildi</span>";
                default:
                    return $"<span class='badge bg-secondary'>{status}</span>";
            }
        }

        private void ShowMessage(string message, string type)
        {
            if (lblMessage != null)
            {
                lblMessage.Text = message;
                lblMessage.CssClass = $"alert alert-{type}";
                lblMessage.Visible = true;
            }
        }
    }
}
