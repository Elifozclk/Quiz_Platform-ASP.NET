<%@ Page Title="Detaylı İstatistikler" Language="C#" MasterPageFile="~/MasterPages/Admin.master" 
    AutoEventWireup="true" CodeBehind="Statistics.aspx.cs" 
    Inherits="QP_WEBPROJECT.vs2.Pages.Admin.Statistics" %>


<asp:Content ID="HelpTitleContent" ContentPlaceHolderID="HelpTitle" runat="server">
    İstatistikler
</asp:Content>

<asp:Content ID="HelpContentMain" ContentPlaceHolderID="HelpContent" runat="server">
    <h4>📊 İstatistikler</h4>
    <p>
        Sistemin genel kullanım verilerini görüntüleyin.
    </p>
    
    <h4>📈 Genel Sayılar</h4>
    <ul>
        <li><strong>Toplam Quiz:</strong> Sistemdeki tüm quiz sayısı</li>
        <li><strong>Yayınlanan Quiz:</strong> Aktif olan quiz'ler</li>
        <li><strong>Taslak Quiz:</strong> Yayınlanmamış quiz'ler</li>
        <li><strong>Toplam Kullanıcı:</strong> Kayıtlı kullanıcı sayısı</li>
        <li><strong>Toplam Çözüm:</strong> Tamamlanan quiz sayısı</li>
    </ul>
    
    <h4>🏆 En Çok Çözülen Quiz'ler</h4>
    <p>Popüler quiz'leri görebilirsiniz:</p>
    <ul>
        <li>Quiz başlığı</li>
        <li>Çözülme sayısı</li>
        <li>Oluşturulma tarihi</li>
    </ul>
    
    <div class="help-tip">
        💡 Popüler quiz'leri inceleyerek yeni fikirler edinebilirsiniz.
    </div>
    
    <h4>👥 Kullanıcı Bilgileri</h4>
    <ul>
        <li><strong>Aktif Kullanıcı:</strong> Son 30 günde giriş yapanlar</li>
        <li><strong>Yeni Üyeler:</strong> Son kayıt olanlar</li>
    </ul>
    
    <h4>📅 Tarih Filtreleme</h4>
    <p>Verileri farklı dönemler için görüntüleyin:</p>
    <ul>
        <li>Son 7 gün</li>
        <li>Son 30 gün</li>
        <li>Tüm zamanlar</li>
    </ul>
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Chart.js -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
    <style>
        .stats-header {
            background: linear-gradient(135deg, #5B0E2D 0%, #8B2248 100%);
            color: white;
            padding: 30px;
            border-radius: 15px;
            margin-bottom: 30px;
            box-shadow: 0 4px 15px rgba(91, 14, 45, 0.3);
        }

        .stats-header h2 {
            margin: 0;
            font-weight: 700;
        }

        /* Accordion Sections */
        .accordion-section {
            background: white;
            border-radius: 12px;
            margin-bottom: 20px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            overflow: hidden;
        }

        .accordion-header {
            background: linear-gradient(135deg, #F6EEF9 0%, #FFF4F8 100%);
            padding: 20px 25px;
            cursor: pointer;
            display: flex;
            justify-content: space-between;
            align-items: center;
            border-bottom: 2px solid #5B0E2D;
            transition: all 0.3s;
        }

        .accordion-header:hover {
            background: linear-gradient(135deg, #F0E4F5 0%, #FFE9F0 100%);
        }

        .accordion-header h4 {
            margin: 0;
            color: #5B0E2D;
            font-weight: 700;
            font-size: 1.3rem;
        }

        .accordion-icon {
            font-size: 1.5rem;
            color: #5B0E2D;
            transition: transform 0.3s;
        }

        .accordion-icon.open {
            transform: rotate(180deg);
        }

        .accordion-body {
            padding: 25px;
            display: none;
        }

        .accordion-body.open {
            display: block;
            animation: slideDown 0.3s ease;
        }

        @keyframes slideDown {
            from {
                opacity: 0;
                max-height: 0;
            }
            to {
                opacity: 1;
                max-height: 1000px;
            }
        }

        /* Stat Cards - Küçültülmüş */
        .stat-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 12px;
            margin-bottom: 20px;
        }

        .stat-card {
            background: linear-gradient(135deg, #F6EEF9 0%, #FFF 100%);
            padding: 15px;
            border-radius: 10px;
            border-left: 4px solid #5B0E2D;
            transition: transform 0.3s;
        }

        .stat-card:hover {
            transform: translateY(-3px);
            box-shadow: 0 6px 15px rgba(91, 14, 45, 0.12);
        }

        .stat-card .icon {
            font-size: 1.5rem;
            color: #5B0E2D;
            margin-bottom: 8px;
        }

        .stat-card .label {
            color: #6b7280;
            font-size: 0.8rem;
            margin-bottom: 6px;
            font-weight: 500;
        }

        .stat-card .value {
            color: #5B0E2D;
            font-size: 1.5rem;
            font-weight: 700;
            line-height: 1;
        }

        .stat-card .sub-value {
            color: #8B2248;
            font-size: 0.75rem;
            margin-top: 6px;
        }

        .stat-card .trend-up {
            color: #10b981;
            font-weight: 600;
        }

        .stat-card .trend-down {
            color: #ef4444;
            font-weight: 600;
        }

        /* Chart Containers */
        .chart-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
            gap: 20px;
            margin-top: 25px;
        }

        .chart-container {
            background: white;
            padding: 20px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }

        .chart-container h5 {
            color: #5B0E2D;
            font-weight: 700;
            margin-bottom: 15px;
            text-align: center;
        }

        .chart-wrapper {
            position: relative;
            height: 300px;
        }

        /* Additional Stats */
        .info-box {
            background: linear-gradient(135deg, #FFF4B0 0%, #FFE9A0 100%);
            padding: 15px;
            border-radius: 10px;
            margin-top: 20px;
            border-left: 4px solid #f59e0b;
        }

        .info-box strong {
            color: #92400e;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Header -->
    <div class="stats-header">
        <h2><i class="fa fa-chart-line"></i> Detaylı İstatistikler</h2>
        <p style="margin: 10px 0 0 0; opacity: 0.9;">Platformunuzun canlı performans ve kullanım verileri</p>
    </div>

    <!-- 1. KULLANICI İSTATİSTİKLERİ -->
    <div class="accordion-section">
        <div class="accordion-header" onclick="toggleAccordion('user-stats')">
            <h4><i class="fa fa-users"></i> Kullanıcı İstatistikleri</h4>
            <i class="fa fa-chevron-down accordion-icon" id="icon-user-stats"></i>
        </div>
        <div class="accordion-body" id="user-stats">
            <!-- Stat Cards -->
            <div class="stat-grid">
                <div class="stat-card">
                    <div class="icon"><i class="fa fa-user-group"></i></div>
                    <div class="label">Toplam Kullanıcı</div>
                    <div class="value"><asp:Label ID="lblTotalUsers" runat="server" Text="0" /></div>
                    <div class="sub-value trend-up">
                        <i class="fa fa-arrow-up"></i>
                        <asp:Label ID="lblUsersThisWeek" runat="server" Text="0" /> bu hafta
                    </div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-circle-check"></i></div>
                    <div class="label">Aktif Kullanıcılar</div>
                    <div class="value"><asp:Label ID="lblActiveUsers" runat="server" Text="0" /></div>
                    <div class="sub-value">Son 30 günde</div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-ban"></i></div>
                    <div class="label">Devre Dışı</div>
                    <div class="value"><asp:Label ID="lblInactiveUsers" runat="server" Text="0" /></div>
                    <div class="sub-value">Pasif hesaplar</div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-cake-candles"></i></div>
                    <div class="label">Ortalama Yaş</div>
                    <div class="value"><asp:Label ID="lblAverageAge" runat="server" Text="N/A" /></div>
                    <div class="sub-value">Platform ortalaması</div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-venus-mars"></i></div>
                    <div class="label">Kadın Kullanıcı</div>
                    <div class="value"><asp:Label ID="lblFemaleUsers" runat="server" Text="0" /></div>
                    <div class="sub-value">
                        <asp:Label ID="lblFemalePercent" runat="server" Text="0" />%
                    </div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-mars"></i></div>
                    <div class="label">Erkek Kullanıcı</div>
                    <div class="value"><asp:Label ID="lblMaleUsers" runat="server" Text="0" /></div>
                    <div class="sub-value">
                        <asp:Label ID="lblMalePercent" runat="server" Text="0" />%
                    </div>
                </div>
            </div>

            <!-- Grafikler -->
            <div class="chart-grid">
                <div class="chart-container">
                    <h5><i class="fa fa-chart-pie"></i> Cinsiyet Dağılımı</h5>
                    <div class="chart-wrapper">
                        <canvas id="genderChart"></canvas>
                    </div>
                </div>

                <div class="chart-container">
                    <h5><i class="fa fa-chart-bar"></i> Yaş Dağılımı</h5>
                    <div class="chart-wrapper">
                        <canvas id="ageChart"></canvas>
                    </div>
                </div>

                <div class="chart-container">
                    <h5><i class="fa fa-chart-line"></i> Haftalık Kayıt Trendi</h5>
                    <div class="chart-wrapper">
                        <canvas id="registrationTrendChart"></canvas>
                    </div>
                </div>
            </div>

            <!-- Hidden data for charts -->
            <asp:HiddenField ID="hdnGenderData" runat="server" />
            <asp:HiddenField ID="hdnAgeData" runat="server" />
            <asp:HiddenField ID="hdnRegistrationData" runat="server" />
        </div>
    </div>

    <!-- 2. QUIZ İSTATİSTİKLERİ -->
    <div class="accordion-section">
        <div class="accordion-header" onclick="toggleAccordion('quiz-stats')">
            <h4><i class="fa fa-clipboard-list"></i> Quiz İstatistikleri</h4>
            <i class="fa fa-chevron-down accordion-icon" id="icon-quiz-stats"></i>
        </div>
        <div class="accordion-body" id="quiz-stats">
            <div class="stat-grid">
                <div class="stat-card">
                    <div class="icon"><i class="fa fa-list"></i></div>
                    <div class="label">Toplam Quiz</div>
                    <div class="value"><asp:Label ID="lblTotalQuizzes" runat="server" Text="0" /></div>
                    <div class="sub-value">
                        <asp:Label ID="lblQuizzesThisMonth" runat="server" Text="0" /> bu ay
                    </div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-circle-check"></i></div>
                    <div class="label">Yayınlanan</div>
                    <div class="value"><asp:Label ID="lblPublishedQuizzes" runat="server" Text="0" /></div>
                    <div class="sub-value">
                        <asp:Label ID="lblPublishRate" runat="server" Text="0" />% oran
                    </div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-file-pen"></i></div>
                    <div class="label">Taslak</div>
                    <div class="value"><asp:Label ID="lblDraftQuizzes" runat="server" Text="0" /></div>
                    <div class="sub-value">Beklemede</div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-question"></i></div>
                    <div class="label">Toplam Soru</div>
                    <div class="value"><asp:Label ID="lblTotalQuestions" runat="server" Text="0" /></div>
                    <div class="sub-value">
                        Ort: <asp:Label ID="lblAvgQuestions" runat="server" Text="0" />
                    </div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-list-check"></i></div>
                    <div class="label">Toplam Seçenek</div>
                    <div class="value"><asp:Label ID="lblTotalOptions" runat="server" Text="0" /></div>
                    <div class="sub-value">
                        Ort: <asp:Label ID="lblAvgOptions" runat="server" Text="0" />
                    </div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-trophy"></i></div>
                    <div class="label">En Popüler</div>
                    <div class="value" style="font-size: 1.2rem;">
                        <asp:Label ID="lblMostPopularQuiz" runat="server" Text="N/A" />
                    </div>
                    <div class="sub-value">
                        <asp:Label ID="lblPopularQuizCount" runat="server" Text="0" /> çözüm
                    </div>
                </div>
            </div>

            <!-- Quiz Grafikler -->
            <div class="chart-grid">
                <div class="chart-container">
                    <h5><i class="fa fa-chart-pie"></i> Quiz Durum Dağılımı</h5>
                    <div class="chart-wrapper">
                        <canvas id="quizStatusChart"></canvas>
                    </div>
                </div>

                <div class="chart-container">
                    <h5><i class="fa fa-chart-bar"></i> Soru Sayısı Dağılımı</h5>
                    <div class="chart-wrapper">
                        <canvas id="questionCountChart"></canvas>
                    </div>
                </div>

                <div class="chart-container">
                    <h5><i class="fa fa-chart-line"></i> En Çok Çözülen Quiz'ler</h5>
                    <div class="chart-wrapper">
                        <canvas id="topQuizzesChart"></canvas>
                    </div>
                </div>
            </div>

            <asp:HiddenField ID="hdnQuizStatusData" runat="server" />
            <asp:HiddenField ID="hdnQuestionCountData" runat="server" />
            <asp:HiddenField ID="hdnTopQuizzesData" runat="server" />
        </div>
    </div>

    <!-- 3. ÇÖZÜM İSTATİSTİKLERİ -->
    <div class="accordion-section">
        <div class="accordion-header" onclick="toggleAccordion('submission-stats')">
            <h4><i class="fa fa-check-circle"></i> Çözüm İstatistikleri</h4>
            <i class="fa fa-chevron-down accordion-icon" id="icon-submission-stats"></i>
        </div>
        <div class="accordion-body" id="submission-stats">
            <div class="stat-grid">
                <div class="stat-card">
                    <div class="icon"><i class="fa fa-clipboard-check"></i></div>
                    <div class="label">Toplam Çözüm</div>
                    <div class="value"><asp:Label ID="lblTotalSubmissions" runat="server" Text="0" /></div>
                    <div class="sub-value trend-up">
                        <i class="fa fa-arrow-up"></i>
                        <asp:Label ID="lblSubmissionsThisWeek" runat="server" Text="0" /> bu hafta
                    </div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-calendar-day"></i></div>
                    <div class="label">Bugün</div>
                    <div class="value"><asp:Label ID="lblTodaySubmissions" runat="server" Text="0" /></div>
                    <div class="sub-value">Günün çözümleri</div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-chart-simple"></i></div>
                    <div class="label">Günlük Ortalama</div>
                    <div class="value"><asp:Label ID="lblAvgDaily" runat="server" Text="0" /></div>
                    <div class="sub-value">Son 30 gün</div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-user-clock"></i></div>
                    <div class="label">Kullanıcı Başına</div>
                    <div class="value"><asp:Label ID="lblAvgPerUser" runat="server" Text="0" /></div>
                    <div class="sub-value">Ortalama çözüm</div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-crown"></i></div>
                    <div class="label">En Aktif Kullanıcı</div>
                    <div class="value" style="font-size: 1.1rem;">
                        <asp:Label ID="lblTopUser" runat="server" Text="N/A" />
                    </div>
                    <div class="sub-value">
                        <asp:Label ID="lblTopUserCount" runat="server" Text="0" /> çözüm
                    </div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-clock"></i></div>
                    <div class="label">Son Çözüm</div>
                    <div class="value" style="font-size: 1rem;">
                        <asp:Label ID="lblLastSubmission" runat="server" Text="N/A" />
                    </div>
                    <div class="sub-value">En son aktivite</div>
                </div>
            </div>

            <!-- Çözüm Grafikler -->
            <div class="chart-grid">
                <div class="chart-container">
                    <h5><i class="fa fa-chart-area"></i> Son 7 Gün Çözüm Trendi</h5>
                    <div class="chart-wrapper">
                        <canvas id="submissionTrendChart"></canvas>
                    </div>
                </div>

                <div class="chart-container">
                    <h5><i class="fa fa-chart-bar"></i> Saatlik Dağılım</h5>
                    <div class="chart-wrapper">
                        <canvas id="hourlyDistributionChart"></canvas>
                    </div>
                </div>

                <div class="chart-container">
                    <h5><i class="fa fa-chart-pie"></i> Haftalık Dağılım</h5>
                    <div class="chart-wrapper">
                        <canvas id="weeklyDistributionChart"></canvas>
                    </div>
                </div>
            </div>

            <asp:HiddenField ID="hdnSubmissionTrendData" runat="server" />
            <asp:HiddenField ID="hdnHourlyData" runat="server" />
            <asp:HiddenField ID="hdnWeeklyData" runat="server" />
        </div>
    </div>

    <!-- 4. SİSTEM İSTATİSTİKLERİ -->
    <div class="accordion-section">
        <div class="accordion-header" onclick="toggleAccordion('system-stats')">
            <h4><i class="fa fa-server"></i> Sistem İstatistikleri</h4>
            <i class="fa fa-chevron-down accordion-icon" id="icon-system-stats"></i>
        </div>
        <div class="accordion-body" id="system-stats">
            <div class="stat-grid">
                <div class="stat-card">
                    <div class="icon"><i class="fa fa-trash-alt"></i></div>
                    <div class="label">Bekleyen Talepler</div>
                    <div class="value"><asp:Label ID="lblPendingDeletes" runat="server" Text="0" /></div>
                    <div class="sub-value">Silme talepleri</div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-check-double"></i></div>
                    <div class="label">Onaylanan Talepler</div>
                    <div class="value"><asp:Label ID="lblApprovedDeletes" runat="server" Text="0" /></div>
                    <div class="sub-value">Tamamlanan</div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-database"></i></div>
                    <div class="label">Veritabanı Boyutu</div>
                    <div class="value" style="font-size: 1.4rem;">
                        <asp:Label ID="lblDatabaseSize" runat="server" Text="N/A" />
                    </div>
                    <div class="sub-value">Toplam kullanım</div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-table"></i></div>
                    <div class="label">Toplam Tablo Sayısı</div>
                    <div class="value"><asp:Label ID="lblTableCount" runat="server" Text="0" /></div>
                    <div class="sub-value">Veritabanı tabloları</div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-shield-halved"></i></div>
                    <div class="label">Admin Sayısı</div>
                    <div class="value"><asp:Label ID="lblAdminCount" runat="server" Text="0" /></div>
                    <div class="sub-value">Yetkili kullanıcılar</div>
                </div>

                <div class="stat-card">
                    <div class="icon"><i class="fa fa-calendar"></i></div>
                    <div class="label">Platform Yaşı</div>
                    <div class="value" style="font-size: 1.2rem;">
                        <asp:Label ID="lblPlatformAge" runat="server" Text="N/A" />
                    </div>
                    <div class="sub-value">İlk kayıt tarihi</div>
                </div>
            </div>

            <!-- Bilgi Kutusu -->
            <div class="info-box">
                <strong><i class="fa fa-info-circle"></i> Sistem Bilgisi:</strong>
                Son veri güncelleme zamanı: <asp:Label ID="lblLastUpdate" runat="server" />
            </div>
        </div>
    </div>

    <script>
        // Accordion Toggle
        function toggleAccordion(id) {
            const body = document.getElementById(id);
            const icon = document.getElementById('icon-' + id);

            if (body.classList.contains('open')) {
                body.classList.remove('open');
                icon.classList.remove('open');
            } else {
                body.classList.add('open');
                icon.classList.add('open');
            }
        }

        // İlk accordion'u aç
        window.addEventListener('load', function () {
            toggleAccordion('user-stats');
        });

        // Chart.js Grafikler
        // (Code-behind'dan JSON data gelecek)
    </script>
</asp:Content>
