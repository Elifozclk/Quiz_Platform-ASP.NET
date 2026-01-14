using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace QP_WEBPROJECT.vs2.Helpers
{
    public static class DbHelper
    {
        private static string ConnectionString =>
            System.Configuration.ConfigurationManager
                .ConnectionStrings["QuizDb"].ConnectionString;

        // 🔌 MySQL bağlantısını açar
        public static MySqlConnection Open()
        {
            var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        // 📋 SELECT sorguları için DataTable döndürür
        public static DataTable GetDataTable(string sql, params MySqlParameter[] parameters)
        {
            using (var conn = Open())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                using (var da = new MySqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
        public static MySqlConnection Open()
        {
            var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public static object Scalar(string sql, params MySqlParameter[] parameters)
        {
            using (var conn = Open())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteScalar();
            }
        }

        // ✏️ INSERT / UPDATE / DELETE sorguları için
        public static int Execute(string sql, params MySqlParameter[] parameters)
        {
            using (var conn = Open())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteNonQuery();
            }
        }

        // 🔢 Tek bir sonuç döner (örnek: SELECT COUNT(*), LAST_INSERT_ID() vb.)
       
    }
}
