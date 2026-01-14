# 🎯 Sosyal İçerik & Quiz Platformu

Video Linki: https://drive.google.com/drive/folders/1eLbGl8x_wDEdTDalK4PM2SPmXPPSoQwl?usp=drive_link
Rapor Sunumu: 

[![.NET](https://img.shields.io/badge/.NET-4.7.2-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?logo=mysql&logoColor=white)](https://www.mysql.com/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

> Kullanıcıların etkileşimli kişilik testleri ve quizler oluşturabileceği, çözebileceği ve sonuçlarını paylaşabileceği modern web platformu.

---

## 📖 İçindekiler

- [Özellikler](#-özellikler)
- [Teknoloji Stack](#-teknoloji-stack)
- [Sistem Mimarisi](#-sistem-mimarisi)
- [Kurulum](#-kurulum)
- [API Dokümantasyonu](#-api-dokümantasyonu)
- [Ekran Görüntüleri](#-ekran-görüntüleri)
- [Proje Videosu](#-proje-videosu)
- [Lisans](#-lisans)

---

## ✨ Özellikler

### 👤 Kullanıcı Özellikleri
- ✅ Kullanıcı kaydı ve giriş (profil resmi upload)
- ✅ Profil yönetimi (bio, ilgi alanları, konum)
- ✅ Şifre değiştirme
- ✅ Quiz çözme ve sonuç görüntüleme
- ✅ Quiz geçmişi
- ✅ Hesap devre dışı bırakma/yeniden aktifleştirme

### 📝 Quiz Özellikleri
- ✅ Kapak görseli ile quiz oluşturma
- ✅ Rich text editor (CKEditor) ile açıklama
- ✅ Metin ve görsel soru/şık desteği
- ✅ JSON-based kişilik puanlama sistemi
- ✅ Multiple sonuç tipi (puan aralıklarına göre)
- ✅ Draft/Published durumları
- ✅ Slug-based URL sistemi

### 🔧 Admin Özellikleri
- ✅ Dashboard ile istatistikler
- ✅ Quiz CRUD işlemleri
- ✅ Kullanıcı yönetimi
- ✅ Silme talepleri yönetimi
- ✅ Cascade delete
- ✅ İstatistiksel raporlama

### 🔒 Güvenlik
- ✅ SHA256 password hashing
- ✅ Parameterized SQL queries (SQL injection koruması)
- ✅ Session-based authentication
- ✅ Role-based access control
- ✅ File upload validation

---

## 🛠 Teknoloji Stack

### Backend
- **ASP.NET 4.7.2** - Web Forms framework
- **Web API 2.0** - RESTful API
- **C#** - Backend programlama dili
- **MySQL 8.0** - İlişkisel veritabanı
- **ADO.NET** - Database access
- **Swagger** - API dokümantasyonu

### Frontend
- **Bootstrap 5** - Responsive CSS framework
- **JavaScript** - Client-side programming
- **jQuery** - DOM manipulation
- **CKEditor** - Rich text editor

---

## 🏗 Sistem Mimarisi

```
┌─────────────────────────────────────────────────┐
│          Presentation Layer (ASPX)              │
│  ┌──────────┬──────────┬────────────────────┐  │
│  │  Public  │   User   │       Admin        │  │
│  │  Pages   │  Pages   │       Pages        │  │
│  └──────────┴──────────┴────────────────────┘  │
└────────────────────┬────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────┐
│             API Layer (Web API 2.0)             │
│                 AuthController                  │
│  ┌──────────────────────────────────────────┐  │
│  │  • Login    • Register    • Profile      │  │
│  │  • Update   • Delete      • Admin Ops    │  │
│  └──────────────────────────────────────────┘  │
└────────────────────┬────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────┐
│         Data Access Layer (DbHelper)            │
│                 ADO.NET + MySQL                 │
└─────────────────────────────────────────────────┘
```

---

## 📦 Kurulum

### Gereksinimler
- Visual Studio 2022 (veya 2019)
- MySQL 8.0 veya üzeri
- IIS Express
- .NET Framework 4.7.2

### Adım 1: Repository'yi Klonlayın
```bash
git clone https://github.com/[USERNAME]/quiz-platform.git
cd quiz-platform
```

### Adım 2: Veritabanını Kurun
1. MySQL Workbench veya phpMyAdmin'i açın
2. `database/web_quiz_platform_db.sql` dosyasını import edin
3. `Web.config` dosyasında connection string'i güncelleyin:

```xml
<connectionStrings>
  <add name="MyDb" 
       connectionString="server=localhost;database=web_quiz_platform_db;uid=root;pwd=;" 
       providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

### Adım 3: Projeyi Açın
1. Visual Studio ile `.sln` dosyasını açın
2. NuGet paketlerini geri yükleyin (Restore NuGet Packages)
3. `Uploads` klasörüne yazma izni verin

### Adım 4: Çalıştırın
```bash
# Visual Studio'da F5 tuşuna basın
# veya
dotnet run
```

### İlk Admin Hesabı
Veritabanında `users` tablosuna manuel admin kullanıcı ekleyin:

```sql
INSERT INTO users (UserName, Email, PasswordHash, Role, IsActive) 
VALUES ('admin', 'admin@example.com', '[SHA256_HASH]', 'admin', 1);
```

---

### Api Blgisi

### Özet Tablo

| # | Method | Endpoint | Açıklama | Auth |
|---|--------|----------|----------|------|
| 1 | POST | `/api/auth/login` | Kullanıcı girişi | ❌ |
| 2 | POST | `/api/auth/register` | Kullanıcı kaydı | ❌ |
| 3 | PUT | `/api/auth/reactivate/{userId}` | Hesabı aktifleştir | ❌ |
| 4 | GET | `/api/auth/user/{id}` | Kullanıcı bilgisi | ✅ |
| 5 | PUT | `/api/auth/user/{id}` | Profil güncelle | ✅ |
| 6 | PUT | `/api/auth/changepassword` | Şifre değiştir | ✅ |
| 7 | GET | `/api/user/submissions/{userId}` | Quiz geçmişi | ✅ |
| 8 | DELETE | `/api/admin/deleterequest/{id}` | Kullanıcıyı sil | 🔒 Admin |
| 9| PUT | `/api/admin/deleterequest/{id}/reject` | Talebi reddet | 🔒 Admin |

---

## 🗄️ Veritabanı Şeması

### Tablolar

| Tablo | Açıklama |
|-------|----------|
| `users` | Kullanıcı bilgileri (profil, şifre hash, rol) |
| `quizzes` | Quiz bilgileri (başlık, açıklama, kapak) |
| `questions` | Quiz soruları (metin/görsel) |
| `options` | Şıklar (metin/görsel, puanlama) |
| `results` | Sonuç tipleri (puan aralıkları) |
| `submissions` | Kullanıcı cevapları |
| `submissionanswers` | Detaylı cevap kayıtları |
| `deleterequests` | Silme talepleri |

### İlişkiler
```
users (1) ─── (N) quizzes
quizzes (1) ─── (N) questions
questions (1) ─── (N) options
quizzes (1) ─── (N) results
users (1) ─── (N) submissions
submissions (1) ─── (N) submissionanswers
```

---

## 📸 Ekran Görüntüleri

### Ana Sayfa
![Ana Sayfa](screenshots/home.png)

### Quiz Çözme
![Quiz Çözme](screenshots/solve-quiz.png)

### Admin Panel
![Admin Panel](screenshots/admin-dashboard.png)

### Quiz Oluşturma
![Quiz Oluşturma](screenshots/quiz-create.png)

---

## 🎥 Proje Videosu

📹 **Detaylı Proje Tanıtım Videosu:** 

Video içeriği: https://drive.google.com/drive/folders/1eLbGl8x_wDEdTDalK4PM2SPmXPPSoQwl?usp=drive_link
- Kod yapısı ve mimari anlatımı
- API endpoints gösterimi
- Veritabanı şeması
- Canlı demo (quiz oluşturma, çözme, sonuç görüntüleme)
- Admin panel özellikleri
- Yapay zeka kullanılan bölümler

---


**Not:** API, veritabanı ve DbHelpers.cs dosyası yapay zeka kullanılmadan, kendi araştırmalarımla geliştirilmiştir.

---

## 📁 Proje Yapısı

```
QP_WEBPROJECT.vs2/
├── Api/
│   └── AuthController.cs           # RESTful API endpoints
├── App_Start/
│   ├── BundleConfig.cs
│   ├── RouteConfig.cs
│   ├── SwaggerConfig.cs
│   └── WebApiConfig.cs
├── MasterPages/
│   ├── Site.Master                 # User master page
│   └── Admin.Master                # Admin master page
├── Pages/
│   ├── Public/
│   │   ├── Default.aspx           # Ana sayfa
│   │   └── SolveQuiz.aspx         # Quiz çözme
│   ├── User/
│   │   ├── Login.aspx
│   │   ├── Register.aspx
│   │   ├── Profile.aspx
│   │   ├── Settings.aspx
│   │   └── UserDashboard.aspx
│   └── Admin/
│       ├── AdminDashboard.aspx
│       ├── QuizCreate.aspx        # Quiz oluşturma wizard
│       ├── QuizManagement.aspx
│       ├── UserManagement.aspx
│       ├── Statistics.aspx
│       └── DeleteRequests.aspx
├── Helpers/
│   └── DbHelper.cs                # Database helper class
├── Uploads/
│   ├── ProfileImages/             # Kullanıcı profil resimleri
│   ├── QuizCovers/                # Quiz kapak görselleri
│   ├── QuestionImages/            # Soru görselleri
│   └── OptionImages/              # Şık görselleri
├── Database/
│   └── web_quiz_platform_db.sql   # Veritabanı scripti
└── Web.config                     # Konfigürasyon dosyası
```

---

## 🧪 Test

### Manual Test
1. Login/Register işlemleri test edildi ✅
2. Quiz oluşturma ve çözme test edildi ✅
3. Admin panel işlemleri test edildi ✅
4. API endpoints Swagger ile test edildi ✅
5. Dosya upload validasyonu test edildi ✅


---

## 🤝 Katkıda Bulunma

1. Bu repository'yi fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'inizi push edin (`git push origin feature/AmazingFeature`)
5. Pull Request oluşturun

---

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

---

## 👨‍💻 Geliştirici

**[Öğrenci Adı]**
- 🎓 Ankara Üniversitesi - Mühendislik Fakültesi
- 📧 Email: ozvelikelif22@gmail.com
- 🔗 LinkedIn: www.linkedin.com/in/elif-özçelik-59177b254

---

## 🙏 Teşekkürler

- Ankara Üniversitesi Mühendislik Fakültesi
- Ağ Tabanlı Teknolojiler ve Uygulamaları Dersi

---

## 📝 Notlar

- Bu proje, Ağ Tabanlı Teknolojiler ve Uygulamaları dersi kapsamında final projesi olarak geliştirilmiştir.
- Proje, ASP.NET Web Forms (2022 ortamı) kullanılarak geliştirilmiştir.
- API dokümantasyonu Swagger ile sağlanmıştır.

---
