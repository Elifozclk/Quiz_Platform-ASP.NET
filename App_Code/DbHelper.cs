using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;

public static class DbHelper
{
    private static string ConnectionString
    {
        get
        {
            var cs = ConfigurationManager.ConnectionStrings["MyDb"];
            if (cs == null)
                throw new System.Exception("QuizDb connection string bulunamadı.");

            return cs.ConnectionString;
        }
    }

    public static MySqlConnection Open()
    {
        var conn = new MySqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }

    public static DataTable Query(string sql, params MySqlParameter[] parameters)
    {
        using (var conn = Open())
        using (var cmd = new MySqlCommand(sql, conn))
        {
            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            using (var da = new MySqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }



    public static int Execute(string sql, params MySqlParameter[] parameters)
    {
        try
        {
            using (var conn = Open())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                int result = cmd.ExecuteNonQuery();

                // Debug için
                System.Diagnostics.Debug.WriteLine($"Execute başarılı: {result} satır etkilendi");

                return result;
            }
        }
        catch (Exception ex)
        {
            // Hatayı logla
            System.Diagnostics.Debug.WriteLine($"DbHelper.Execute HATA: {ex.Message}");
            throw; // Hatayı yukarı fırlat
        }
    }

 


    public static long ExecuteInsert(string sql, params MySqlParameter[] parameters)
    {
        try
        {
            using (var conn = Open())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                cmd.ExecuteNonQuery();

                // AYNI connection'da LastInsertedId'yi al
                return cmd.LastInsertedId;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ExecuteInsert Error: {ex.Message}");
            throw;
        }
    }

    public static object Scalar(string sql, params MySqlParameter[] parameters)
    {
        using (var conn = Open())
        using (var cmd = new MySqlCommand(sql, conn))
        {
            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            return cmd.ExecuteScalar();
        }
    }
}
