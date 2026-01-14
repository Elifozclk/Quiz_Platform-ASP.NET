using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QP_WEBPROJECT.vs2.Pages.Admin
{
    public partial class UserManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadUsers();
        }

        private void LoadUsers(string keyword = "")
        {
            string sql = "SELECT Id, UserName, Email, Role, IF(IsActive=1, 'Evet', 'Hayır') AS IsActiveText FROM Users";

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql += " WHERE UserName LIKE @kw OR Email LIKE @kw";
            }

            sql += " ORDER BY CreatedAt DESC";

            var param = new MySqlParameter("@kw", "%" + keyword + "%");
            DataTable dt = string.IsNullOrWhiteSpace(keyword)
                ? DbHelper.Query(sql)
                : DbHelper.Query(sql, param);

            gvUsers.DataSource = dt;
            gvUsers.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            LoadUsers(keyword);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            LoadUsers();
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            int userId = Convert.ToInt32(gvUsers.DataKeys[index].Value);
            int currentAdminId = Convert.ToInt32(Session["UserId"]);

            if (userId == currentAdminId)
            {
                lblMessage.Text = "⚠️ Kendi hesabınız üzerinde bu işlemi yapamazsınız.";
                return;
            }

            switch (e.CommandName)
            {
                case "ToggleActive":
                    ToggleUserActive(userId);
                    break;
                case "ToggleRole":
                    ToggleUserRole(userId);
                    break;
                case "DeleteUser":
                    DeleteUser(userId);
                    break;
            }

            LoadUsers();
        }

        private void ToggleUserActive(int userId)
        {
            string sql = "UPDATE Users SET IsActive = IF(IsActive=1, 0, 1) WHERE Id = @id";
            DbHelper.Execute(sql, new MySqlParameter("@id", userId));
        }

        private void ToggleUserRole(int userId)
        {
            string sql = @"
                UPDATE Users
                SET Role = CASE WHEN Role = 'admin' THEN 'user' ELSE 'admin' END
                WHERE Id = @id";
            DbHelper.Execute(sql, new MySqlParameter("@id", userId));
        }

        private void DeleteUser(int userId)
        {
            // Ek güvenlik: admin rolündeki kullanıcı silinmesin
            var roleCheck = DbHelper.Query("SELECT Role FROM Users WHERE Id = @id", new MySqlParameter("@id", userId));
            if (roleCheck.Rows.Count > 0 && roleCheck.Rows[0]["Role"].ToString() == "admin")
                return;

            DbHelper.Execute("DELETE FROM Users WHERE Id = @id", new MySqlParameter("@id", userId));
        }

        protected void gvUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string isActive = DataBinder.Eval(e.Row.DataItem, "IsActiveText").ToString();

                if (isActive == "Hayır")
                {
                    e.Row.ForeColor = System.Drawing.Color.Gray;
                }
            }
        }
    }
}
