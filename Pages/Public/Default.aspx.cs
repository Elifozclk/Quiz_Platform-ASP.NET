using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace QP_WEBPROJECT.vs2.Pages.Public
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadQuizzes();
        }
        protected string SafeSnippet(object descObj)
        {
            var text = descObj?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(text))
                return "Hemen çözmeye başla!";

            // 1) HTML taglerini temizle
            text = Regex.Replace(text, "<.*?>", string.Empty);

            // 2) HTML entity decode (örn: &nbsp;)
            text = HttpUtility.HtmlDecode(text);

            // 3) Kırp
            const int maxLen = 100;
            if (text.Length > maxLen)
                text = text.Substring(0, maxLen) + "...";

            // 4) Güvenli bas (etiket üretmesin)
            return HttpUtility.HtmlEncode(text);
        }
        private void LoadQuizzes(string keyword = "")
        {
            try
            {
                string sql = @"SELECT 
                Id, 
                Title, 
                Description, 
                Slug, 
                CoverImageUrl, 
                EstimatedTime, 
                CreatedAt
               FROM Quizzes 
               WHERE Status='Published'";

                // Arama varsa ekle
                if (!string.IsNullOrEmpty(keyword))
                {
                    sql += " AND (Title LIKE @keyword OR Description LIKE @keyword)";
                }

                sql += " ORDER BY CreatedAt DESC LIMIT 20";

                DataTable dt;
                if (!string.IsNullOrEmpty(keyword))
                {
                    dt = DbHelper.Query(sql, new MySqlParameter("@keyword", $"%{keyword}%"));
                }
                else
                {
                    dt = DbHelper.Query(sql);
                }

                if (dt.Rows.Count > 0)
                {
                    rptQuizzes.DataSource = dt;
                    rptQuizzes.DataBind();
                    pnlNoResult.Visible = false;
                }
                else
                {
                    rptQuizzes.DataSource = null;
                    rptQuizzes.DataBind();
                    pnlNoResult.Visible = true;
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda log tutabilirsiniz
                pnlNoResult.Visible = true;
            }
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadQuizzes(txtSearch.Text.Trim());
        }
    }
}