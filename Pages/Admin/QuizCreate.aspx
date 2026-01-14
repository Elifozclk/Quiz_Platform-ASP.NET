<%@ Page Title="Quiz Oluştur" Language="C#" MasterPageFile="~/MasterPages/Admin.master"
    AutoEventWireup="true" CodeBehind="QuizCreate.aspx.cs"
    Inherits="QP_WEBPROJECT.vs2.Pages.Admin.QuizCreate" 
    ValidateRequest="false" 
    EnableViewState="true" %>


<asp:Content ID="HelpTitleContent" ContentPlaceHolderID="HelpTitle" runat="server">
    Quiz Oluşturma
</asp:Content>

<asp:Content ID="HelpContentMain" ContentPlaceHolderID="HelpContent" runat="server">
    <h4>🎨 Quiz Nasıl Oluşturulur?</h4>
    <p>
        Quiz oluşturma 3 panelden oluşur. Her paneli sırayla doldurun.
    </p>
    
    <h4>1️⃣ Panel 1: Temel Bilgiler</h4>
    <ul>
        <li><strong>Başlık:</strong> Quiz'in adı (örn: "Hangi Hayvan Ruhlusun?")</li>
        <li><strong>Açıklama:</strong> Kısa tanıtım metni</li>
        <li><strong>Tahmini Süre:</strong> Ortalama tamamlanma süresi (örn: "5 dakika")</li>
        <li><strong>Quiz Tipi:</strong> "personality" seçin</li>
        <li><strong>Kapak Görseli:</strong> PNG/JPG, max 5MB</li>
    </ul>
    
    <div class="help-tip">
        💡 Başlık soru şeklinde olursa daha ilgi çekici olur!
    </div>
    
    <h4>2️⃣ Panel 2: Sonuçlar</h4>
    <p>En az 2 sonuç tanımlayın:</p>
    <ul>
        <li><strong>Başlık:</strong> Sonucun adı (örn: "Cesur Aslan")</li>
        <li><strong>Açıklama:</strong> Sonuç açıklaması (zengin metin editörü)</li>
        <li><strong>Etiket:</strong> Benzersiz kısa kod (örn: "brave-lion")</li>
        <li><strong>Görsel:</strong> PNG/JPG, max 5MB</li>
    </ul>
    
    <div class="help-warning">
        ⚠️ En az 2 sonuç olmalı! Her etiket benzersiz olmalı.
    </div>
    
    <h4>3️⃣ Panel 3: Sorular</h4>
    <p>En az 3 soru ekleyin:</p>
    
    <p><strong>Soru Ekleme:</strong></p>
    <ul>
        <li><strong>Soru Metni:</strong> Soru cümlesi</li>
        <li><strong>Görsel:</strong> (Opsiyonel) PNG/JPG, max 5MB</li>
    </ul>
    
    <p><strong>Seçenek Ekleme:</strong></p>
    <ul>
        <li><strong>Seçenek Metni:</strong> Cevap seçeneği</li>
        <li><strong>Puan Ataması:</strong> Her seçenek hangi sonuçlara kaç puan verir</li>
    </ul>
    
    <div class="help-tip">
        💡 <strong>Puan Sistemi:</strong><br/>
        • Her seçenek 1 veya daha fazla sonuca puan verir<br/>
        • En yüksek puan alan sonuç kullanıcıya gösterilir<br/>
        • Örnek: "Kitap okurum" → Bilge: 2 puan, Sakin: 1 puan
    </div>
    
    <h4>📏 Minimum Gereksinimler</h4>
    <ul>
        <li>✅ En az <strong>2 sonuç</strong></li>
        <li>✅ En az <strong>3 soru</strong></li>
        <li>✅ Her soruda en az <strong>2 seçenek</strong></li>
        <li>✅ Her seçeneğe en az <strong>1 puan</strong></li>
    </ul>
    
    <h4>💾 Kaydetme</h4>
    <ol>
        <li>Tüm panelleri doldurun</li>
        <li>"Kaydet" butonuna tıklayın</li>
        <li>Quiz otomatik <strong>Taslak</strong> olarak kaydedilir</li>
        <li>"Görüntüle" ile test edin</li>
        <li>Quiz Yönetimi'nden "Yayınla"</li>
    </ol>
    
    <h4>🖼️ Görsel Yükleme</h4>
    <ul>
        <li><strong>Format:</strong> PNG, JPG, JPEG, GIF, WEBP</li>
        <li><strong>Boyut:</strong> Maksimum 5MB</li>
        <li><strong>Önerilen:</strong> Kapak 1200x630px, Sonuç 600x600px</li>
    </ul>
    
    <h4>✏️ Zengin Metin Editörü</h4>
    <p>Açıklama alanlarında kullanabilirsiniz:</p>
    <ul>
        <li>Kalın, İtalik, Altı Çizili</li>
        <li>Madde işareti ve numaralı listeler</li>
        <li>Bağlantı ekleme</li>
    </ul>
    
    <h4>🚫 Sık Yapılan Hatalar</h4>
    <ul>
        <li>❌ Sonuçları tanımlamadan soru eklemek</li>
        <li>❌ Seçeneklere puan atamamak</li>
        <li>❌ Aynı etiketi iki kez kullanmak</li>
        <li>❌ Test etmeden yayınlamak</li>
    </ul>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- CKEditor -->
    <script src="https://cdn.ckeditor.com/4.22.1/standard/ckeditor.js"></script>
    
    <!-- Font Awesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />

    <style>
    :root {
        --soft-purple: #a78bfa;
        --soft-pink: #f9a8d4;
        --soft-blue: #93c5fd;
        --soft-green: #86efac;
        --soft-orange: #fdba74;
        --soft-gray: #9ca3af;
        --text-dark: #374151;
        --text-light: #6b7280;
        --bg-cream: #faf8f5;
        --bg-white: #ffffff;
        --border-soft: #e5e7eb;
    }

    body {
        background: linear-gradient(135deg, #fef3f8 0%, #f0f4ff 50%, #fef9f3 100%);
        font-family: 'Inter', 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    }

    .quiz-wrapper {
        max-width: 1300px;
        margin: 20px auto;
        padding: 0 20px;
    }

    .quiz-header {
        background: linear-gradient(135deg, rgba(167, 139, 250, 0.15) 0%, rgba(249, 168, 212, 0.15) 100%);
        border-radius: 24px;
        padding: 40px;
        text-align: center;
        margin-bottom: 30px;
        border: 1px solid rgba(167, 139, 250, 0.2);
        backdrop-filter: blur(10px);
    }

    .quiz-header h2 {
        color: var(--text-dark);
        font-size: 2rem;
        font-weight: 600;
        margin: 0 0 10px 0;
    }

    .quiz-header p {
        color: var(--text-light);
        font-size: 1rem;
        margin: 0;
    }

    .step-container {
        background: var(--bg-white);
        border-radius: 20px;
        padding: 25px;
        margin-bottom: 25px;
        box-shadow: 0 2px 10px rgba(0, 0, 0, 0.03);
        border: 1px solid var(--border-soft);
    }

    .step-indicator {
        display: flex;
        justify-content: space-between;
        gap: 12px;
        position: relative;
    }

    .step-indicator::before {
        content: '';
        position: absolute;
        top: 35px;
        left: 5%;
        right: 5%;
        height: 3px;
        background: linear-gradient(90deg, var(--soft-purple) 0%, var(--soft-pink) 50%, var(--soft-blue) 100%);
        opacity: 0.2;
        z-index: 0;
    }

    .step {
        flex: 1;
        padding: 18px 12px;
        text-align: center;
        background: var(--bg-cream);
        border-radius: 16px;
        cursor: pointer;
        transition: all 0.3s ease;
        border: 2px solid transparent;
        position: relative;
        z-index: 1;
    }

    .step i {
        font-size: 1.3rem;
        display: block;
        margin-bottom: 8px;
        color: var(--soft-gray);
        transition: all 0.3s ease;
    }

    .step span {
        font-size: 0.85rem;
        font-weight: 500;
        color: var(--text-light);
        display: block;
        transition: all 0.3s ease;
    }

    .step.active {
        background: var(--bg-white);
        border-color: var(--soft-purple);
        box-shadow: 0 4px 12px rgba(167, 139, 250, 0.15);
        transform: translateY(-3px);
    }

    .step.active i {
        color: var(--soft-purple);
    }

    .step.active span {
        color: var(--text-dark);
        font-weight: 600;
    }

    .step.completed {
        background: rgba(134, 239, 172, 0.1);
        border-color: var(--soft-green);
    }

    .step.completed i {
        color: var(--soft-green);
    }

    .step:hover:not(.active) {
        background: var(--bg-white);
        transform: translateY(-2px);
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
    }

    .panel-section {
        display: none;
    }

    .panel-section.active {
        display: block;
        animation: softFadeIn 0.4s ease;
    }

    @keyframes softFadeIn {
        from {
            opacity: 0;
            transform: translateY(10px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }

    .soft-card {
        background: var(--bg-white);
        border-radius: 20px;
        padding: 30px;
        margin-bottom: 20px;
        box-shadow: 0 2px 10px rgba(0, 0, 0, 0.03);
        border: 1px solid var(--border-soft);
        transition: all 0.3s ease;
    }

    .soft-card:hover {
        box-shadow: 0 4px 16px rgba(0, 0, 0, 0.06);
    }

    .card-title {
        font-size: 1.25rem;
        font-weight: 600;
        color: var(--text-dark);
        margin-bottom: 20px;
        display: flex;
        align-items: center;
        gap: 10px;
    }

    .card-title i {
        color: var(--soft-purple);
    }

    .form-label {
        font-weight: 500;
        color: var(--text-dark);
        margin-bottom: 8px;
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 0.9rem;
    }

    .form-label i {
        color: var(--soft-purple);
        font-size: 0.9rem;
    }

    .form-control, .form-select {
        border: 2px solid var(--border-soft);
        border-radius: 12px;
        padding: 10px 14px;
        transition: all 0.3s ease;
        background: var(--bg-cream);
        color: var(--text-dark);
    }

    .form-control:focus, .form-select:focus {
        border-color: var(--soft-purple);
        box-shadow: 0 0 0 4px rgba(167, 139, 250, 0.08);
        outline: none;
        background: var(--bg-white);
    }

    .btn {
        padding: 10px 20px;
        border-radius: 12px;
        font-weight: 600;
        transition: all 0.3s ease;
        border: none;
        cursor: pointer;
        display: inline-flex;
        align-items: center;
        gap: 8px;
        font-size: 0.9rem;
        color: white;
    }

    .btn-primary {
        background: linear-gradient(135deg, #a78bfa 0%, #f472b6 100%);
    }

    .btn-primary:hover {
        background: linear-gradient(135deg, #9575cd 0%, #ec4899 100%);
        transform: translateY(-2px);
        box-shadow: 0 4px 12px rgba(167, 139, 250, 0.4);
    }

    .btn-success {
        background: linear-gradient(135deg, #22c55e 0%, #16a34a 100%);
    }

    .btn-success:hover {
        background: linear-gradient(135deg, #16a34a 0%, #15803d 100%);
        transform: translateY(-2px);
        box-shadow: 0 4px 12px rgba(34, 197, 94, 0.4);
    }

    .btn-warning {
        background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
    }

    .btn-warning:hover {
        background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
        transform: translateY(-2px);
        box-shadow: 0 4px 12px rgba(251, 191, 36, 0.4);
    }

    .btn-danger {
        background: linear-gradient(135deg, #f87171 0%, #ef4444 100%);
    }

    .btn-danger:hover {
        background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
        transform: translateY(-2px);
        box-shadow: 0 4px 12px rgba(239, 68, 68, 0.4);
    }

    .btn-secondary {
        background: #6b7280;
        color: white;
    }

    .btn-secondary:hover {
        background: #4b5563;
        transform: translateY(-2px);
        box-shadow: 0 4px 12px rgba(107, 114, 128, 0.4);
    }

    .btn-lg {
        padding: 14px 28px;
        font-size: 1rem;
    }

    .btn-sm {
        padding: 6px 12px;
        font-size: 0.85rem;
    }

    .icon-btn {
        width: 32px;
        height: 32px;
        padding: 0;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        border-radius: 10px;
    }

.table {
    border-radius: 14px;
    overflow: hidden;
    border: 1px solid var(--border-soft);
    background: white;
}

.table thead {
    background: linear-gradient(135deg, rgba(167, 139, 250, 0.1) 0%, rgba(249, 168, 212, 0.1) 100%);
}

.table thead th {
    padding: 16px;
    font-weight: 600;
    border: none;
    font-size: 0.9rem;
    color: var(--text-dark);
}

.table tbody tr {
    transition: all 0.2s ease;
}

.table tbody tr:hover {
    background: rgba(167, 139, 250, 0.03);
}

.table tbody td {
    padding: 16px;
    vertical-align: middle;
    border-bottom: 1px solid var(--border-soft);
    color: var(--text-dark);
    font-size: 0.9rem;
}

.badge {
    padding: 8px 16px;
    border-radius: 20px;
    font-weight: 600;
    font-size: 0.85rem;
    display: inline-flex;
    align-items: center;
    gap: 5px;
    min-width: 100px;
    justify-content: center;
}

.badge.bg-success {
    background: #10b981;
    color: white;
}

.badge.bg-warning {
    background: #f59e0b;
    color: white;
}

.badge.bg-danger {
    background: #ef4444;
    color: white;
}

.badge.bg-secondary {
    background: #6b7280;
    color: white;
}

.table .btn {
    margin: 2px;
}

.table .icon-btn {
    width: 36px;
    height: 36px;
    margin: 2px;
}

.table .btn-primary {
    background: linear-gradient(135deg, #8b5cf6 0%, #ec4899 100%);
    color: white;
    font-weight: 600;
    padding: 8px 16px;
    white-space: nowrap;
    font-size: 0.85rem;
}

.table .btn-success {
    background: #10b981;
    color: white;
    font-weight: 600;
    padding: 8px 16px;
}

.table .btn-warning {
    background: #f59e0b;
    color: white;
    font-weight: 600;
    padding: 8px 16px;
}

.table .btn-danger {
    background: #ef4444;
    color: white;
    font-weight: 600;
}

.table .btn-secondary {
    background: #6b7280;
    color: white;
    font-weight: 600;
}

.alert {
    border-radius: 14px;
    padding: 14px 18px;
    margin: 15px 0;
    border: 1px solid;
    animation: softSlideIn 0.4s ease;
    display: flex;
    align-items: center;
    gap: 10px;
}

@keyframes softSlideIn {
    from {
        opacity: 0;
        transform: translateX(-10px);
    }
    to {
        opacity: 1;
        transform: translateX(0);
    }
}

.alert-success {
    background: rgba(134, 239, 172, 0.1);
    border-color: var(--soft-green);
    color: #059669;
}

.alert-warning {
    background: rgba(253, 186, 116, 0.1);
    border-color: var(--soft-orange);
    color: #d97706;
}

.alert-danger {
    background: rgba(252, 165, 165, 0.1);
    border-color: #fca5a5;
    color: #dc2626;
}

.alert-info {
    background: rgba(147, 197, 253, 0.1);
    border-color: var(--soft-blue);
    color: #2563eb;
}

#scoreInputsContainer {
    max-height: 280px;
    overflow-y: auto;
    padding: 10px;
    border-radius: 12px;
    background: var(--bg-cream);
    border: 1px solid var(--border-soft);
}

.score-item {
    background: var(--bg-white);
    padding: 12px;
    border-radius: 10px;
    margin-bottom: 10px;
    border: 1px solid var(--border-soft);
}

.section-purple {
    background: linear-gradient(135deg, rgba(167, 139, 250, 0.05) 0%, rgba(249, 168, 212, 0.05) 100%);
    border: 1px solid rgba(167, 139, 250, 0.15);
}

.section-green {
    background: linear-gradient(135deg, rgba(134, 239, 172, 0.05) 0%, rgba(110, 231, 183, 0.05) 100%);
    border: 1px solid rgba(134, 239, 172, 0.15);
}

.section-orange {
    background: linear-gradient(135deg, rgba(253, 186, 116, 0.05) 0%, rgba(251, 191, 36, 0.05) 100%);
    border: 1px solid rgba(253, 186, 116, 0.15);
}

.section-blue {
    background: linear-gradient(135deg, rgba(147, 197, 253, 0.05) 0%, rgba(96, 165, 250, 0.05) 100%);
    border: 1px solid rgba(147, 197, 253, 0.15);
}

.navigation-buttons {
    display: flex;
    gap: 12px;
    align-items: center;
    padding-top: 25px;
    margin-top: 25px;
    border-top: 1px solid var(--border-soft);
}

.form-check {
    padding: 10px 0;
}

.form-check-input {
    width: 18px;
    height: 18px;
    border-radius: 6px;
    border: 2px solid var(--border-soft);
}

.form-check-input:checked {
    background-color: var(--soft-purple);
    border-color: var(--soft-purple);
}

.result-tag-small {
    display: inline-block;
    background: rgba(167, 139, 250, 0.15);
    color: var(--soft-purple);
    padding: 3px 10px;
    border-radius: 12px;
    font-size: 0.75rem;
    font-weight: 600;
    margin: 2px;
    border: 1px solid rgba(167, 139, 250, 0.3);
}

.result-tags-container {
    display: flex;
    flex-wrap: wrap;
    gap: 4px;
}

/* Önizleme Stilleri */
    .preview-item {
        margin-bottom: 15px;
    }

    .preview-item strong {
        color: var(--text-dark);
        font-weight: 600;
        margin-right: 10px;
    }

    /* Sonuç Kartları */
    .result-preview-card {
        background: white;
        border: 2px solid var(--border-soft);
        border-radius: 16px;
        padding: 20px;
        text-align: center;
        transition: all 0.3s ease;
        height: 100%;
    }

    .result-preview-card:hover {
        transform: translateY(-3px);
        box-shadow: 0 4px 12px rgba(167, 139, 250, 0.15);
    }

    .result-preview-card .result-icon {
        font-size: 3rem;
        margin-bottom: 10px;
    }

    .result-preview-card h6 {
        color: var(--text-dark);
        font-weight: 600;
        margin: 10px 0;
    }

    .result-preview-card .badge {
        font-size: 0.75rem;
        padding: 4px 10px;
    }

    .result-preview-card p {
        font-size: 0.85rem;
        color: var(--text-light);
        margin-top: 10px;
    }

    /* Soru Kartları */
    .question-preview-card {
        background: white;
        border: 2px solid var(--border-soft);
        border-radius: 16px;
        padding: 20px;
        margin-bottom: 20px;
    }

    .question-preview-card .question-number {
        display: inline-block;
        background: var(--soft-purple);
        color: white;
        padding: 5px 12px;
        border-radius: 15px;
        font-size: 0.85rem;
        font-weight: 600;
        margin-bottom: 10px;
    }

    .question-preview-card .question-text {
        font-size: 1.1rem;
        font-weight: 600;
        color: var(--text-dark);
        margin: 10px 0;
    }

    /* Seçenek Kartları */
    .option-preview-card {
        background: var(--bg-cream);
        border: 2px solid var(--border-soft);
        border-radius: 12px;
        padding: 15px;
        margin-bottom: 10px;
        transition: all 0.2s ease;
    }

    .option-preview-card:hover {
        background: white;
        border-color: var(--soft-purple);
    }

    .option-preview-card .option-number {
        display: inline-block;
        background: var(--soft-purple);
        color: white;
        width: 30px;
        height: 30px;
        border-radius: 50%;
        text-align: center;
        line-height: 30px;
        font-weight: 600;
        margin-right: 10px;
    }

    .option-preview-card .option-text {
        font-weight: 500;
        color: var(--text-dark);
    }

    .option-preview-card .option-scores {
        font-size: 0.8rem;
        color: var(--text-light);
        margin-top: 5px;
        padding-top: 5px;
        border-top: 1px solid var(--border-soft);
    }

    .option-preview-card img {
        max-width: 100%;
        height: auto;
        border-radius: 8px;
        margin: 10px 0;
    }

    /* ==================== BİLGİLENDİRME PANELİ STİLLERİ ==================== */
.info-panel-container {
    margin-bottom: 25px;
}

.info-toggle-btn {
    width: 100%;
    background: linear-gradient(135deg, rgba(147, 197, 253, 0.1) 0%, rgba(167, 139, 250, 0.1) 100%);
    border: 2px solid var(--soft-blue);
    border-radius: 16px;
    padding: 16px 24px;
    display: flex;
    align-items: center;
    gap: 12px;
    cursor: pointer;
    transition: all 0.3s ease;
    font-weight: 600;
    color: var(--text-dark);
    font-size: 1rem;
}

.info-toggle-btn:hover {
    background: linear-gradient(135deg, rgba(147, 197, 253, 0.2) 0%, rgba(167, 139, 250, 0.2) 100%);
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(147, 197, 253, 0.3);
}

.info-toggle-btn i:first-child {
    color: var(--soft-blue);
    font-size: 1.3rem;
}

.info-toggle-btn span {
    flex: 1;
    text-align: left;
}

.info-toggle-btn .toggle-icon {
    transition: transform 0.3s ease;
    color: var(--soft-purple);
}

.info-toggle-btn.active .toggle-icon {
    transform: rotate(180deg);
}

.info-panel {
    margin-top: 12px;
    animation: softSlideDown 0.4s ease;
}

@keyframes softSlideDown {
    from {
        opacity: 0;
        transform: translateY(-10px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.info-content {
    background: white;
    border: 2px solid var(--border-soft);
    border-radius: 16px;
    padding: 30px;
}

.info-content h5 {
    color: var(--text-dark);
    font-weight: 600;
    margin-bottom: 20px;
    display: flex;
    align-items: center;
    gap: 10px;
}

.info-content h5 i {
    color: var(--soft-blue);
}

.info-section {
    margin-bottom: 20px;
    padding-bottom: 20px;
    border-bottom: 1px solid var(--border-soft);
}

.info-section:last-of-type {
    border-bottom: none;
    margin-bottom: 0;
    padding-bottom: 0;
}

.info-section h6 {
    color: var(--text-dark);
    font-weight: 600;
    margin-bottom: 12px;
    display: flex;
    align-items: center;
    gap: 8px;
}

.info-section h6 i {
    color: var(--soft-purple);
}

.info-section ul {
    margin: 0;
    padding-left: 25px;
}

.info-section li {
    margin-bottom: 8px;
    color: var(--text-dark);
    line-height: 1.6;
}

.info-section li strong {
    color: var(--soft-purple);
}

.info-section ul ul {
    margin-top: 8px;
}

/* ==================== GÖRSEL ÖNİZLEME STİLLERİ ==================== */
.image-upload-container {
    position: relative;
}

.image-preview-wrapper {
    margin-top: 12px;
    display: none;
    animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
    from {
        opacity: 0;
    }
    to {
        opacity: 1;
    }
}

.image-preview-box {
    position: relative;
    border: 2px dashed var(--border-soft);
    border-radius: 12px;
    padding: 12px;
    background: var(--bg-cream);
}

.image-preview-box.has-image {
    border-style: solid;
    border-color: var(--soft-green);
    background: rgba(134, 239, 172, 0.05);
}

.preview-image-container {
    position: relative;
    display: inline-block;
    max-width: 100%;
}

.preview-image {
    max-width: 100%;
    max-height: 300px;
    border-radius: 8px;
    object-fit: contain;
    display: block;
}

.image-info {
    margin-top: 8px;
    display: flex;
    align-items: center;
    gap: 12px;
    flex-wrap: wrap;
    font-size: 0.85rem;
}

.image-info-item {
    display: flex;
    align-items: center;
    gap: 5px;
    color: var(--text-light);
}

.image-info-item i {
    color: var(--soft-purple);
}

.image-actions {
    margin-top: 12px;
    display: flex;
    gap: 8px;
}

.btn-remove-image {
    background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
    color: white;
    border: none;
    padding: 8px 16px;
    border-radius: 8px;
    cursor: pointer;
    font-size: 0.85rem;
    font-weight: 600;
    display: inline-flex;
    align-items: center;
    gap: 6px;
    transition: all 0.3s ease;
}

.btn-remove-image:hover {
    background: linear-gradient(135deg, #dc2626 0%, #b91c1c 100%);
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(239, 68, 68, 0.4);
}

.btn-change-image {
    background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
    color: white;
    border: none;
    padding: 8px 16px;
    border-radius: 8px;
    cursor: pointer;
    font-size: 0.85rem;
    font-weight: 600;
    display: inline-flex;
    align-items: center;
    gap: 6px;
    transition: all 0.3s ease;
}

.btn-change-image:hover {
    background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(59, 130, 246, 0.4);
}

.file-validation-message {
    margin-top: 8px;
    padding: 10px 14px;
    border-radius: 8px;
    font-size: 0.85rem;
    display: none;
    animation: slideIn 0.3s ease;
}

@keyframes slideIn {
    from {
        opacity: 0;
        transform: translateX(-10px);
    }
    to {
        opacity: 1;
        transform: translateX(0);
    }
}

.file-validation-message.error {
    background: rgba(239, 68, 68, 0.1);
    border: 1px solid rgba(239, 68, 68, 0.3);
    color: #dc2626;
}

.file-validation-message.success {
    background: rgba(134, 239, 172, 0.1);
    border: 1px solid rgba(134, 239, 172, 0.3);
    color: #059669;
}

.file-validation-message i {
    margin-right: 6px;
}

/* Drag & Drop Stili */
.form-control.drag-over {
    border-color: var(--soft-purple);
    background: rgba(167, 139, 250, 0.05);
}

.existing-image-indicator {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    margin-top: 8px;
    padding: 8px 12px;
    background: rgba(134, 239, 172, 0.1);
    border: 1px solid rgba(134, 239, 172, 0.3);
    border-radius: 8px;
    font-size: 0.85rem;
    color: #059669;
}

.existing-image-indicator i {
    color: var(--soft-green);
}



</style>



<div class="quiz-wrapper">
        <!-- Header -->
        <div class="quiz-header">
            <h2>✨ Quiz Yönetim Paneli</h2>
            <p>Eğlenceli ve ilgi çekici quiz'ler oluşturun</p>
        </div>

        <!-- ==================== BİLGİLENDİRME PANELİ ==================== -->
<div class="info-panel-container">
    <button type="button" class="info-toggle-btn" onclick="toggleInfoPanel()">
        <i class="fas fa-question-circle"></i>
        <span>Quiz Nasıl Oluşturulur?</span>
        <i class="fas fa-chevron-down toggle-icon"></i>
    </button>
    
    <div class="info-panel" id="infoPanel" style="display: none;">
        <div class="info-content">
            <h5><i class="fas fa-lightbulb"></i> Quiz Oluşturma Rehberi</h5>
            
            <div class="info-section">
                <h6><i class="fas fa-info-circle"></i> Adım 1: Quiz Bilgileri</h6>
                <ul>
                    <li><strong>Başlık:</strong> Quiz'inizin çekici bir başlığını girin</li>
                    <li><strong>Açıklama:</strong> Quiz hakkında kısa bir tanım yazın</li>
                    <li><strong>Kapak Görseli:</strong> 
                        <ul>
                            <li>Maksimum boyut: 5MB</li>
                            <li>İzin verilen formatlar: JPG, PNG, GIF, WEBP</li>
                            <li>Önerilen boyut: 1200x630 px</li>
                        </ul>
                    </li>
                </ul>
            </div>

            <div class="info-section">
                <h6><i class="fas fa-trophy"></i> Adım 2: Sonuçlar</h6>
                <ul>
                    <li>En az 2, en fazla 10 farklı sonuç ekleyin</li>
                    <li>Her sonuç için benzersiz bir <strong>etiket</strong> kullanın (örn: idealist, realist)</li>
                    <li>Sonuç görselleri opsiyoneldir ancak kullanıcı deneyimini artırır</li>
                    <li><strong>Emoji:</strong> Her sonuç için karakteristik bir emoji ekleyebilirsiniz</li>
                </ul>
            </div>

            <div class="info-section">
                <h6><i class="fas fa-question"></i> Adım 3: Sorular ve Seçenekler</h6>
                <ul>
                    <li><strong>Soru Görseli:</strong> Görsel sorular için fotoğraf ekleyin</li>
                    <li><strong>Seçenekler:</strong> Her soru için 2-6 arası seçenek ekleyin</li>
                    <li><strong>Puanlama:</strong> Her seçeneğe sonuçlara göre puan verin</li>
                    <li><strong>Seçenek Görselleri:</strong> Görsel seçenekler daha etkilidir</li>
                </ul>
            </div>

            <div class="info-section">
                <h6><i class="fas fa-eye"></i> Adım 4: Önizleme ve Yayınlama</h6>
                <ul>
                    <li>Quiz'inizi yayınlamadan önce tüm detayları kontrol edin</li>
                    <li><strong>Taslak:</strong> Henüz hazır değilse taslak olarak saklayın</li>
                    <li><strong>Yayınla:</strong> Hazır olduğunda quiz'i yayınlayın</li>
                </ul>
            </div>

            <div class="alert alert-info mt-3 mb-0">
                <i class="fas fa-images"></i>
                <strong>Görsel İpuçları:</strong>
                <br/>• Tüm görseller otomatik olarak önizlenir
                <br/>• Görseli değiştirmek için yeni dosya seçin
                <br/>• Düzenleme sırasında mevcut görseller korunur
            </div>
        </div>
    </div>
</div>


        <!-- Quiz Seçim Paneli -->
        <div class="quiz-selector-panel">
            <div class="soft-card">
                <div class="row align-items-center">
                    <div class="col-md-4">
                        <h5 class="mb-0" style="color: var(--text-dark); font-weight: 600;">
                            <i class="fas fa-tasks"></i> Quiz İşlemleri
                        </h5>
                    </div>
                    <div class="col-md-8">
                        <div class="d-flex gap-2 justify-content-end">
                            <asp:Button ID="btnNewQuiz" runat="server" 
                                Text="Yeni Quiz Oluştur" 
                                CssClass="btn btn-success" 
                                OnClick="btnNewQuiz_Click" />
                            
                            <asp:Button ID="btnShowDrafts" runat="server" 
                                Text="Taslakları Görüntüle" 
                                CssClass="btn btn-warning" 
                                OnClick="btnShowDrafts_Click" />
                            
                            <asp:Button ID="btnShowPublished" runat="server" 
                                Text="Yayınlananları Görüntüle" 
                                CssClass="btn btn-primary" 
                                OnClick="btnShowPublished_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Quiz Liste Paneli -->
        <asp:Panel ID="pnlQuizList" runat="server" Visible="false" CssClass="quiz-list-panel">
            <div class="soft-card section-blue">
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <h5 class="mb-0" style="color: var(--text-dark); font-weight: 600;">
                        <asp:Label ID="lblQuizListTitle" runat="server" />
                    </h5>
                    <asp:Button ID="btnCloseList" runat="server" 
                        Text="Kapat" 
                        CssClass="btn btn-sm btn-secondary" 
                        OnClick="btnCloseList_Click" />
                </div>

                <!-- Arama Bölümü -->
                <div class="search-section mb-4">
                    <div class="soft-card" style="background: linear-gradient(135deg, rgba(147, 197, 253, 0.1) 0%, rgba(167, 139, 250, 0.1) 100%);">
                        <div class="row align-items-end">
                            <div class="col-md-4">
                                <label class="form-label">
                                    <i class="fas fa-search"></i> Arama Terimi
                                </label>
                                <asp:TextBox ID="txtSearch" runat="server" 
                                    CssClass="form-control" 
                                    placeholder="Başlık, açıklama veya sonuç ara..." />
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">
                                    <i class="fas fa-filter"></i> Arama Alanı
                                </label>
                                <asp:DropDownList ID="ddlSearchField" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="all" Selected="True">Tümünde Ara</asp:ListItem>
                                    <asp:ListItem Value="title">Sadece Başlık</asp:ListItem>
                                    <asp:ListItem Value="description">Sadece Açıklama</asp:ListItem>
                                    <asp:ListItem Value="results">Sadece Sonuç Etiketleri</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-2">
                                <label class="form-label">
                                    <i class="fas fa-sort"></i> Sıralama
                                </label>
                                <asp:DropDownList ID="ddlSortBy" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="date_desc" Selected="True">Yeni → Eski</asp:ListItem>
                                    <asp:ListItem Value="date_asc">Eski → Yeni</asp:ListItem>
                                    <asp:ListItem Value="title_asc">A → Z</asp:ListItem>
                                    <asp:ListItem Value="title_desc">Z → A</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-3">
                                <div class="d-flex gap-2">
                                    <asp:Button ID="btnSearch" runat="server" 
                                        Text="Ara" 
                                        CssClass="btn btn-primary flex-grow-1" 
                                        OnClick="btnSearch_Click" />
                                    <asp:Button ID="btnClearSearch" runat="server" 
                                        Text="Temizle" 
                                        CssClass="btn btn-secondary" 
                                        OnClick="btnClearSearch_Click" />
                                </div>
                            </div>
                        </div>
                        
                        <asp:Panel ID="pnlSearchInfo" runat="server" Visible="false" CssClass="mt-3">
                            <div class="alert alert-info mb-0" style="padding: 10px 15px;">
                                <i class="fas fa-info-circle"></i>
                                <asp:Label ID="lblSearchInfo" runat="server" />
                            </div>
                        </asp:Panel>
                    </div>
                </div>

                <!-- Quiz Listesi GridView -->
                <asp:GridView ID="gvQuizList" runat="server" CssClass="table" 
                    AutoGenerateColumns="False" EmptyDataText="Quiz bulunamadı"
                    OnRowCommand="gvQuizList_RowCommand" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-Width="60px" />
                        <asp:TemplateField HeaderText="Başlık">
                            <ItemTemplate>
                                <div>
                                    <strong><%# Eval("Title") %></strong>
                                    <br />
                                    <small class="text-muted">
                                        <%# !string.IsNullOrEmpty(Eval("Description").ToString()) 
                                            ? (Eval("Description").ToString().Length > 100 
                                                ? Eval("Description").ToString().Substring(0, 100) + "..." 
                                                : Eval("Description").ToString())
                                            : "" %>
                                    </small>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sonuçlar" ItemStyle-Width="200px">
                            <ItemTemplate>
                                <%# GetResultTags(Eval("Id")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Durum" ItemStyle-Width="120px" ItemStyle-CssClass="text-center">
                            <ItemTemplate>
                                <%# Eval("Status").ToString() == "Published" 
                                    ? "<span class='badge bg-success'><i class='fas fa-check'></i> Yayında</span>" 
                                    : "<span class='badge bg-warning'><i class='fas fa-edit'></i> Taslak</span>" %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CreatedAt" HeaderText="Tarih" 
                            ItemStyle-Width="130px" DataFormatString="{0:dd.MM.yyyy}" />
                        <asp:TemplateField HeaderText="Sorular" ItemStyle-Width="80px" ItemStyle-CssClass="text-center">
                            <ItemTemplate>
                                <%# GetQuestionCount(Eval("Id")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                       <asp:TemplateField HeaderText="İşlem" ItemStyle-Width="200px" ItemStyle-CssClass="text-center">
    <ItemTemplate>
        <div class="d-flex gap-1 justify-content-center flex-wrap">
            <asp:LinkButton ID="btnEditQuiz" runat="server" 
                CommandName="EditQuiz" 
                CommandArgument='<%# Eval("Id") %>'
                CssClass="btn btn-primary btn-sm"
                ToolTip="Düzenle">
                <i class="fas fa-edit"></i>
            </asp:LinkButton>
            <asp:LinkButton ID="btnDuplicateQuiz" runat="server" 
                CommandName="DuplicateQuiz" 
                CommandArgument='<%# Eval("Id") %>'
                CssClass="btn btn-success btn-sm"
                ToolTip="Kopyala">
                <i class="fas fa-copy"></i>
            </asp:LinkButton>
            <asp:LinkButton ID="btnDeleteQuiz" runat="server" 
                CommandName="DeleteQuiz" 
                CommandArgument='<%# Eval("Id") %>'
                CssClass="btn btn-danger btn-sm"
                OnClientClick="return confirm('Quiz ve tüm soruları silinecek! Emin misiniz?');"
                ToolTip="Sil">
                <i class="fas fa-trash"></i>
            </asp:LinkButton>
        </div>
    </ItemTemplate>
</asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

        <!-- Step Container -->
        <div class="step-container">
            <div class="step-indicator">
                <div class="step active" data-step="1">
                    <i class="fas fa-info-circle"></i>
                    <span>Quiz Bilgileri</span>
                </div>
                <div class="step" data-step="2">
                    <i class="fas fa-trophy"></i>
                    <span>Sonuçlar</span>
                </div>
                <div class="step" data-step="3">
                    <i class="fas fa-list-check"></i>
                    <span>Sorular</span>
                </div>
                <div class="step" data-step="4">
                    <i class="fas fa-eye"></i>
                    <span>Önizleme</span>
                </div>
            </div>
        </div>
    </div>

    <!-- Hidden Fields -->
    <asp:HiddenField ID="hfCurrentQuizId" runat="server" Value="0" />
    <asp:HiddenField ID="hfCurrentStep" runat="server" Value="1" />
    <asp:HiddenField ID="hfScoreData" runat="server" Value="{}" />

    <!-- ==================== PANEL 1: Quiz Bilgileri ==================== -->
    <div id="panel1" class="panel-section active">
        <div class="soft-card">
            <div class="card-title">
                <i class="fas fa-pencil"></i>
                Temel Bilgiler
            </div>
            
            <div class="row">
                <div class="col-md-6">
                    <div class="mb-3">
                        <label class="form-label">
                            <i class="fas fa-heading"></i> Quiz Başlığı
                        </label>
                        <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" 
                            placeholder="Hangi karakter tipisiniz?" />
                    </div>
                    
                    <div class="mb-3">
                        <label class="form-label">
                            <i class="fas fa-align-left"></i> Açıklama
                        </label>
                        <asp:TextBox ID="txtDescription" runat="server" 
                            CssClass="form-control" 
                            TextMode="MultiLine" Rows="6" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">
                            <i class="fas fa-clock"></i> Tahmini Süre (dakika)
                        </label>
                        <asp:TextBox ID="txtEstimatedTime" runat="server" CssClass="form-control" 
                            TextMode="Number" Text="5" />
                    </div>
                </div>

               <div class="col-md-6">
    <label class="form-label">
        <i class="fas fa-image"></i> Kapak Görseli
    </label>
    <div class="image-upload-container">
        <asp:FileUpload ID="fuCoverImage" runat="server" CssClass="form-control" accept="image/*" />
        <small class="text-muted">
            Maksimum: 5MB | JPG, PNG, GIF, WEBP | Önerilen: 1200x630px
        </small>
        
        <!-- Mevcut görsel varsa göster -->
        <asp:Panel ID="pnlExistingCover" runat="server" Visible="false" CssClass="existing-image-indicator">
            <i class="fas fa-check-circle"></i>
            <asp:Label ID="lblExistingCover" runat="server" Text="Mevcut kapak görseli var" />
        </asp:Panel>
        
        <!-- Önizleme alanı -->
        <div id="previewCoverImage" class="image-preview-wrapper">
            <div class="image-preview-box">
                <div class="preview-image-container"></div>
                <div class="image-info"></div>
                <div class="file-validation-message"></div>
                <div class="image-actions">
                    <button type="button" class="btn-remove-image">
                        <i class="fas fa-trash"></i> Kaldır
                    </button>
                    <button type="button" class="btn-change-image">
                        <i class="fas fa-sync"></i> Değiştir
                    </button>
                </div>
            </div>
        </div>
    </div>
</div>

                    <div class="row mb-3">
    <div class="col-md-6" style="display:none;">
        <label class="form-label">
            <i class="fas fa-shapes"></i> Quiz Tipi
        </label>
        <asp:DropDownList ID="ddlQuizType" runat="server" CssClass="form-select">
            <asp:ListItem Value="PersonalityClassic" Selected="True">Kişilik Testi</asp:ListItem>
            <asp:ListItem Value="EliminationPair">Bu mu Şu mu</asp:ListItem>
        </asp:DropDownList>
    </div>

    <div class="col-md-12">
        <label class="form-label">
            <i class="fas fa-list"></i> Seçenek Tipi
        </label>
        <asp:DropDownList ID="ddlOptionType" runat="server" CssClass="form-select">
            <asp:ListItem Value="TextOnly">Sadece Metin</asp:ListItem>
            <asp:ListItem Value="ImageOnly">Sadece Görsel</asp:ListItem>
            <asp:ListItem Value="Mixed" Selected="True">Karma</asp:ListItem>
        </asp:DropDownList>
    </div>
</div>

                   <div class="row mb-3" style="display:none;">
    <div class="col-md-6">
        <label class="form-label">
            <i class="fas fa-palette"></i> Tema
        </label>
        <asp:DropDownList ID="ddlTheme" runat="server" CssClass="form-select">
            <asp:ListItem Value="light" Selected="True">Aydınlık</asp:ListItem>
            <asp:ListItem Value="dark">Karanlık</asp:ListItem>
            <asp:ListItem Value="colorful">Renkli</asp:ListItem>
        </asp:DropDownList>
    </div>

    <div class="col-md-6">
        <label class="form-label">
            <i class="fas fa-paint-brush"></i> Arkaplan
        </label>
        <input id="txtBgColor" name="txtBgColor" type="color" value="#ffffff" 
            class="form-control" style="height: 45px;" />
    </div>
</div>

                    <div class="form-check">
                        <asp:CheckBox ID="chkAnonymousAllowed" runat="server" CssClass="form-check-input" Checked="true" />
                        <label class="form-check-label">Anonim katılıma izin ver</label>
                    </div>

                    <div class="form-check">
                        <asp:CheckBox ID="chkMultipleAttempts" runat="server" CssClass="form-check-input" Checked="true" />
                        <label class="form-check-label">Birden fazla deneme</label>
                    </div>

                    <div class="form-check">
                        <asp:CheckBox ID="chkIsActive" runat="server" CssClass="form-check-input" />
                        <label class="form-check-label">Hemen yayınla</label>
                    </div>
                </div>
            </div>

            <div class="navigation-buttons">
                <asp:Button ID="btnSaveQuiz" runat="server" Text="Kaydet ve Devam Et" 
    CssClass="btn btn-success btn-lg" OnClick="btnSaveQuiz_Click" 
    OnClientClick="return saveCKEditorData();" />
                <asp:Label ID="lblMessage1" runat="server" />
            </div>
        </div>
    
<!-- ==================== PANEL 2: Sonuçlar ==================== -->
    <div id="panel2" class="panel-section">
        <div class="soft-card section-green">
            <div class="card-title">
                <i class="fas fa-trophy"></i>
                Olası Sonuçlar
            </div>
            
            <div class="alert alert-info">
                <i class="fas fa-lightbulb"></i>
                Quiz'den çıkabilecek sonuçları ekleyin (örn: İdealist, Realist, Maceracı)
            </div>

            <div class="row">
                <div class="col-md-5">
                    <div class="soft-card section-orange">
                        <h6 class="mb-3" style="font-weight: 600; color: var(--text-dark);">
                            <i class="fas fa-plus"></i> Yeni Sonuç
                        </h6>
                        
                        <div class="mb-3">
                            <label class="form-label">
                                <i class="fas fa-tag"></i> Başlık
                            </label>
                            <asp:TextBox ID="txtResultTitle" runat="server" CssClass="form-control" 
                                placeholder="Örn: İdealist" />
                        </div>
                        
                        <div class="mb-3">
                            <label class="form-label">
                                <i class="fas fa-hashtag"></i> Etiket
                            </label>
                            <asp:TextBox ID="txtResultTag" runat="server" CssClass="form-control" 
                                placeholder="idealist" />
                            <small class="text-muted">Küçük harf, boşluksuz</small>
                        </div>
                        
                        <div class="mb-3">
    <label class="form-label">
        <i class="fas fa-image"></i> Görsel
    </label>
    <div class="image-upload-container">
        <asp:FileUpload ID="fuResultImage" runat="server" CssClass="form-control" accept="image/*" />
        <small class="text-muted">
            Maksimum: 5MB | JPG, PNG, GIF, WEBP | Önerilen: 600x600px
        </small>
        
        <!-- Düzenleme modunda mevcut görsel -->
        <asp:Panel ID="pnlExistingResultImage" runat="server" Visible="false" CssClass="existing-image-indicator">
            <i class="fas fa-check-circle"></i>
            <asp:Label ID="lblExistingResultImage" runat="server" Text="Mevcut görsel var" />
        </asp:Panel>
        
        <!-- Önizleme alanı -->
        <div id="previewResultImage" class="image-preview-wrapper">
            <div class="image-preview-box">
                <div class="preview-image-container"></div>
                <div class="image-info"></div>
                <div class="file-validation-message"></div>
                <div class="image-actions">
                    <button type="button" class="btn-remove-image">
                        <i class="fas fa-trash"></i> Kaldır
                    </button>
                    <button type="button" class="btn-change-image">
                        <i class="fas fa-sync"></i> Değiştir
                    </button>
                </div>
            </div>
        </div>
    </div>
</div>

                        <div class="mb-3">
                            <label class="form-label">
                                <i class="fas fa-smile"></i> Emoji/İkon
                            </label>
                            <asp:TextBox ID="txtResultEmoji" runat="server" CssClass="form-control" 
                                placeholder="🦁 veya 🐱" MaxLength="10" />
                            <small class="text-muted">Emoji veya ikon (opsiyonel)</small>
                        </div>
                        
                        <div class="mb-3">
                            <label class="form-label">
                                <i class="fas fa-comment"></i> Açıklama
                            </label>
                            <asp:TextBox ID="txtResultDescription" runat="server" 
                                CssClass="form-control" TextMode="MultiLine" Rows="3" />
                        </div>
                        
                        <div class="d-flex gap-2">
                            <asp:Button ID="btnAddResult" runat="server" Text="Ekle" 
                                CssClass="btn btn-primary flex-grow-1" OnClick="btnAddResult_Click" />
                            <asp:Button ID="btnCancelResultEdit" runat="server" Text="İptal" 
                                CssClass="btn btn-secondary" OnClick="btnCancelResultEdit_Click" 
                                Visible="false" />
                        </div>
                        <asp:Label ID="lblMessage2" runat="server" CssClass="d-block mt-2 text-center" />
                    </div>
                </div>
                
                <div class="col-md-7">
                    <asp:GridView ID="gvResults" runat="server" CssClass="table" 
                        AutoGenerateColumns="False" EmptyDataText="Henüz sonuç eklenmedi"
                        OnRowCommand="gvResults_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-Width="60px" />
<asp:BoundField DataField="Title" HeaderText="Başlık" />
<asp:BoundField DataField="Tag" HeaderText="Etiket" ItemStyle-Width="130px" />

<asp:TemplateField HeaderText="Görsel" ItemStyle-Width="80px" ItemStyle-CssClass="text-center">
    <ItemTemplate>
        <%# GetResultImagePreview(Eval("ImageUrl"), Eval("IconEmoji")) %>
    </ItemTemplate>
</asp:TemplateField>


                            <asp:TemplateField HeaderText="İşlem" ItemStyle-Width="150px" ItemStyle-CssClass="text-center">
    <ItemTemplate>
        <div class="d-flex gap-1 justify-content-center">
            <asp:LinkButton ID="btnEditResult" runat="server" 
                CommandName="EditResult" 
                CommandArgument='<%# Eval("Id") %>'
                CssClass="btn btn-sm btn-warning"
                ToolTip="Düzenle">
                <i class="fas fa-edit"></i>
            </asp:LinkButton>
            <asp:LinkButton ID="btnDeleteResult" runat="server" 
                CommandName="DeleteResult" 
                CommandArgument='<%# Eval("Id") %>'
                CssClass="btn btn-sm btn-danger"
                OnClientClick="return confirm('Bu sonucu silmek istediğinizden emin misiniz?');"
                ToolTip="Sil">
                <i class="fas fa-trash"></i>
            </asp:LinkButton>
        </div>
    </ItemTemplate>
</asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <div class="navigation-buttons">
                <button type="button" class="btn btn-secondary" onclick="goToStep(1)">
                    <i class="fas fa-arrow-left"></i> Geri
                </button>
                <asp:Button ID="btnGoToQuestions" runat="server" Text="Devam Et" 
                    CssClass="btn btn-primary" OnClick="btnGoToQuestions_Click" />
            </div>
        </div>
    </div>

    <!-- ==================== PANEL 3: Sorular ==================== -->
    <div id="panel3" class="panel-section">
        
        <asp:HiddenField ID="hfCurrentQuestionId" runat="server" Value="0" />
        
        <div class="soft-card section-purple">
            <div class="card-title">
                <i class="fas fa-list-check"></i>
                Sorular
            </div>
            
            <!-- Soru Listesi -->
            <div class="mb-4">
                <asp:GridView ID="gvQuestions" runat="server" CssClass="table" 
                    AutoGenerateColumns="False" EmptyDataText="Henüz soru eklenmedi"
                    OnRowCommand="gvQuestions_RowCommand" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="OrderNo" HeaderText="#" ItemStyle-Width="50px" ItemStyle-CssClass="text-center" />
                        <asp:BoundField DataField="Text" HeaderText="Soru" />
                        <asp:TemplateField HeaderText="Durum" ItemStyle-Width="120px" ItemStyle-CssClass="text-center">
    <ItemTemplate>
        <%# Convert.ToInt32(Eval("OptionCount")) == 0 
            ? "<span class='badge bg-danger'>Seçenek yok</span>" 
            : Convert.ToInt32(Eval("OptionCount")) == 1 
                ? "<span class='badge bg-warning'>1 seçenek</span>" 
                : "<span class='badge bg-success'>" + Eval("OptionCount") + " seçenek</span>" %>
    </ItemTemplate>
</asp:TemplateField>
                        <asp:TemplateField HeaderText="Sıra" ItemStyle-Width="90px" ItemStyle-CssClass="text-center">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnMoveUp" runat="server" 
                                    CommandName="MoveUpQuestion" 
                                    CommandArgument='<%# Eval("Id") %>'
                                    CssClass="btn btn-sm btn-secondary icon-btn">
                                    <i class="fas fa-arrow-up"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnMoveDown" runat="server" 
                                    CommandName="MoveDownQuestion" 
                                    CommandArgument='<%# Eval("Id") %>'
                                    CssClass="btn btn-sm btn-secondary icon-btn">
                                    <i class="fas fa-arrow-down"></i>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Görsel" ItemStyle-Width="80px" ItemStyle-CssClass="text-center">
    <ItemTemplate>
        <%# GetQuestionImagePreview(Eval("ImageUrl")) %>
    </ItemTemplate>
</asp:TemplateField>

                        <asp:TemplateField HeaderText="İşlem" ItemStyle-Width="220px" ItemStyle-CssClass="text-center">
                            <ItemTemplate>
                                <div class="d-flex gap-1 justify-content-center flex-wrap">
                                    <asp:LinkButton ID="btnManageOptions" runat="server" 
                                        CommandName="ManageOptions" 
                                        CommandArgument='<%# Eval("Id") %>'
                                        CssClass="btn btn-primary btn-sm">
                                        <i class="fas fa-cog"></i> Seçenekler
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnEditQuestion" runat="server" 
                                        CommandName="EditQuestion" 
                                        CommandArgument='<%# Eval("Id") %>'
                                        CssClass="btn btn-warning btn-sm icon-btn"
                                        ToolTip="Düzenle">
                                        <i class="fas fa-edit"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDeleteQuestion" runat="server" 
                                        CommandName="DeleteQuestion" 
                                        CommandArgument='<%# Eval("Id") %>'
                                        CssClass="btn btn-danger btn-sm icon-btn"
                                        OnClientClick="return confirm('Silmek istediğinize emin misiniz?');"
                                        ToolTip="Sil">
                                        <i class="fas fa-trash"></i>
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <hr style="border-color: var(--border-soft);" />

            <!-- Yeni Soru Ekle -->
            <div class="soft-card section-green mb-4">
                <h6 style="font-weight: 600; color: var(--text-dark); margin-bottom: 15px;">
                    <i class="fas fa-plus"></i> Yeni Soru Ekle
                </h6>
                
                <div class="row">
                    <div class="col-md-8">
                        <div class="mb-3">
                            <label class="form-label">
                                <i class="fas fa-question"></i> Soru Metni
                            </label>
                            <asp:TextBox ID="txtQuestionText" runat="server" CssClass="form-control" 
                                TextMode="MultiLine" Rows="2"
                                placeholder="Boş zamanlarınızda ne yapmayı seversiniz?" />
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="mb-3">
    <label class="form-label">
        <i class="fas fa-image"></i> Soru Görseli
    </label>
    <div class="image-upload-container">
        <asp:FileUpload ID="fuQuestionImage" runat="server" CssClass="form-control" accept="image/*" />
        <small class="text-muted">
            Maksimum: 5MB | JPG, PNG, GIF, WEBP | Önerilen: 800x600px
        </small>
        
        <!-- Düzenleme modunda mevcut görsel -->
<asp:Panel ID="pnlExistingQuestionImage" runat="server" Visible="false" CssClass="existing-image-indicator">
    <i class="fas fa-check-circle"></i>
    <asp:Label ID="lblExistingQuestionImage" runat="server" Text="Mevcut soru görseli var" />
    
    <div style="margin-top: 10px; padding: 10px; background: #fff3cd; border: 2px solid #ffc107; border-radius: 8px;">
        <asp:CheckBox ID="chkRemoveQuestionImage" runat="server" CssClass="form-check-input" style="width: 18px; height: 18px; margin-right: 8px;" />
        <label style="color: #856404; font-weight: 600; cursor: pointer; font-size: 14px;">
            <i class="fas fa-trash-alt" style="color: #dc3545;"></i> Bu görseli kalıcı olarak kaldır
        </label>
    </div>
</asp:Panel>
        
        <!-- Önizleme alanı -->
        <div id="previewQuestionImage" class="image-preview-wrapper">
            <div class="image-preview-box">
                <div class="preview-image-container"></div>
                <div class="image-info"></div>
                <div class="file-validation-message"></div>
                <div class="image-actions">
                    <button type="button" class="btn-remove-image">
                        <i class="fas fa-trash"></i> Kaldır
                    </button>
                    <button type="button" class="btn-change-image">
                        <i class="fas fa-sync"></i> Değiştir
                    </button>
                </div>
            </div>
        </div>
    </div>
</div>
                </div>
                
                <div class="row">
                    <div class="col-md-6">
                        <label class="form-label">
                            <i class="fas fa-sort"></i> Sıra No
                        </label>
                        <asp:TextBox ID="txtQuestionOrder" runat="server" CssClass="form-control" 
                            TextMode="Number" Text="1" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">
                            <i class="fas fa-star"></i> Puan Çarpanı
                        </label>
                        <asp:TextBox ID="txtPointMultiplier" runat="server" CssClass="form-control" 
                            Text="1.0" />
                    </div>
                </div>
                
                <hr style="border-color: var(--border-soft); margin: 15px 0;" />

                <div class="d-flex gap-2">
                    <asp:Button ID="btnAddQuestion" runat="server" Text="Soru Ekle" 
                        CssClass="btn btn-success flex-grow-1" OnClick="btnAddQuestion_Click" />
                    <asp:Button ID="btnCancelQuestionEdit" runat="server" Text="İptal" 
                        CssClass="btn btn-secondary" OnClick="btnCancelQuestionEdit_Click" 
                        Visible="false" />
                </div>
                <asp:Label ID="lblMessage3" runat="server" CssClass="ms-2" />
            </div>

            <!-- Seçenek Yönetimi Paneli -->
            <asp:Panel ID="pnlManageOptions" runat="server" Visible="false" CssClass="soft-card section-orange">
                <h6 style="font-weight: 600; color: var(--text-dark); margin-bottom: 10px;">
                    <i class="fas fa-cog"></i> Seçenek Yönetimi
                </h6>
                <div class="alert alert-info">
                    <strong>Soru:</strong> <asp:Label ID="lblCurrentQuestion" runat="server" />
                </div>

                <div class="row">
                    <div class="col-md-6">
                        <div class="soft-card" style="background: var(--bg-cream);">
                            <h6 class="mb-3" style="font-weight: 600;">
                                <i class="fas fa-plus"></i> Yeni Seçenek
                            </h6>
                            
                            <div class="alert alert-light py-2">
                                <small><strong>Tip:</strong> <asp:Label ID="lblQuizOptionType" runat="server" /></small>
                            </div>

                            <div class="mb-3">
                                <label class="form-label">
                                    <i class="fas fa-font"></i> Metin
                                </label>
                                <asp:TextBox ID="txtOptionText" runat="server" CssClass="form-control" />
                            </div>

                            <div class="mb-3">
    <label class="form-label">
        <i class="fas fa-image"></i> Seçenek Görseli
    </label>
    <div class="image-upload-container">
        <asp:FileUpload ID="fuOptionImage" runat="server" CssClass="form-control" accept="image/*" />
        <small class="text-muted">
            Maksimum: 5MB | JPG, PNG, GIF, WEBP | Önerilen: 400x400px
        </small>
        
       <!-- Düzenleme modunda mevcut görsel -->
<asp:Panel ID="pnlExistingOptionImage" runat="server" Visible="false" CssClass="existing-image-indicator">
    <i class="fas fa-check-circle"></i>
    <asp:Label ID="lblExistingOptionImage" runat="server" Text="Mevcut seçenek görseli var" />
    
    <div style="margin-top: 10px; padding: 10px; background: #fff3cd; border: 2px solid #ffc107; border-radius: 8px;">
        <asp:CheckBox ID="chkRemoveOptionImage" runat="server" CssClass="form-check-input" style="width: 18px; height: 18px; margin-right: 8px;" />
        <label style="color: #856404; font-weight: 600; cursor: pointer; font-size: 14px;">
            <i class="fas fa-trash-alt" style="color: #dc3545;"></i> Bu görseli kalıcı olarak kaldır
        </label>
    </div>
</asp:Panel>
        
        <!-- Önizleme alanı -->
        <div id="previewOptionImage" class="image-preview-wrapper">
            <div class="image-preview-box">
                <div class="preview-image-container"></div>
                <div class="image-info"></div>
                <div class="file-validation-message"></div>
                <div class="image-actions">
                    <button type="button" class="btn-remove-image">
                        <i class="fas fa-trash"></i> Kaldır
                    </button>
                    <button type="button" class="btn-change-image">
                        <i class="fas fa-sync"></i> Değiştir
                    </button>
                </div>
            </div>
        </div>
    </div>
</div>

                            <div class="mb-3">
                                <label class="form-label">
                                    <i class="fas fa-sort"></i> Sıra
                                </label>
                                <asp:TextBox ID="txtOptionOrder" runat="server" CssClass="form-control" 
                                    TextMode="Number" Text="1" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label">
                                    <i class="fas fa-star"></i> Puan Dağılımı
                                </label>
                                <div id="scoreInputsContainer"></div>
                                <small class="text-muted">0-10 arası puan verin</small>
                            </div>

                            <asp:Button ID="btnAddOption" runat="server" Text="Seçenek Ekle" 
                                CssClass="btn btn-success w-100 mb-2" OnClick="btnAddOption_Click" />
                            
                            <asp:Button ID="btnCancelEdit" runat="server" Text="İptal" 
                                CssClass="btn btn-secondary w-100" OnClick="btnCancelEdit_Click" 
                                Visible="false" />
                            
                            <asp:Label ID="lblMessage4" runat="server" CssClass="d-block text-center mt-2" />
                        </div>
                    </div>

                    <div class="col-md-6">
                        <h6 class="mb-3" style="font-weight: 600;">
                            <i class="fas fa-list"></i> Eklenen Seçenekler
                        </h6>
                        
                        <asp:GridView ID="gvOptions" runat="server" CssClass="table table-sm" 
                            AutoGenerateColumns="False" EmptyDataText="Henüz seçenek yok"
                            OnRowCommand="gvOptions_RowCommand">
                            <Columns>
    <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-Width="60px" />
    <asp:BoundField DataField="OrderNo" HeaderText="Sıra" ItemStyle-Width="60px" ItemStyle-CssClass="text-center" />
    <asp:BoundField DataField="Text" HeaderText="Metin" />
    
    <asp:TemplateField HeaderText="Görsel" ItemStyle-Width="80px" ItemStyle-CssClass="text-center">
        <ItemTemplate>
            <%# GetOptionImagePreview(Eval("ImageUrl")) %>
        </ItemTemplate>
    </asp:TemplateField>
    
    <asp:TemplateField HeaderText="İşlem" ItemStyle-Width="150px" ItemStyle-CssClass="text-center">
        <ItemTemplate>
            <asp:LinkButton ID="btnEditOption" runat="server" 
                CommandName="EditOption" 
                CommandArgument='<%# Eval("Id") %>'
                CssClass="btn btn-primary btn-sm"
                ToolTip="Düzenle">
                <i class="fas fa-edit"></i>
            </asp:LinkButton>
            
            <asp:LinkButton ID="btnDeleteOption" runat="server" 
                CommandName="DeleteOption" 
                CommandArgument='<%# Eval("Id") %>'
                CssClass="btn btn-danger btn-sm"
                ToolTip="Sil"
                OnClientClick="return confirm('Bu seçeneği silmek istediğinizden emin misiniz?');">
                <i class="fas fa-trash"></i>
            </asp:LinkButton>
        </ItemTemplate>
    </asp:TemplateField>
</Columns>
                        </asp:GridView>

                        <div class="alert alert-success">
                            <i class="fas fa-check"></i>
                            En az 2 seçenek ekleyin
                            <br />
                            <asp:Button ID="btnCloseOptions" runat="server" Text="Tamam" 
                                CssClass="btn btn-sm btn-primary mt-2" OnClick="btnCloseOptions_Click" />
                        </div>
                    </div>
                </div>
            </asp:Panel>

        </div>

        <div class="navigation-buttons">
            <button type="button" class="btn btn-secondary btn-lg" onclick="goToStep(2)">
                <i class="fas fa-arrow-left"></i> Geri
            </button>
            <asp:Button ID="btnGoToPreview" runat="server" Text="Tamamla" 
                CssClass="btn btn-success btn-lg" OnClick="btnGoToPreview_Click" />
            <asp:Label ID="lblMessage3Final" runat="server" CssClass="ms-3" />
        </div>
    </div>


<!-- ==================== PANEL 4: Önizleme ==================== -->

<div id="panel4" class="panel-section">
    
    <div class="soft-card section-blue">
        <div class="card-title">
            <i class="fas fa-eye"></i>
            Quiz Önizleme
        </div>
        
        <div class="alert alert-info">
            <i class="fas fa-info-circle"></i>
            Quiz'inizi yayınlamadan önce kontrol edin
        </div>

        <!-- Quiz Bilgileri -->
        <div class="soft-card mb-4">
            <h5 class="mb-3" style="color: var(--text-dark); font-weight: 600;">
                Quiz Bilgileri
            </h5>
            
            <div class="row">
                <div class="col-md-8">
                    <p><strong>Başlık:</strong> <asp:Label ID="lblPreviewTitle" runat="server" /></p>
<p><strong>Açıklama:</strong> <asp:Label ID="lblPreviewDescription" runat="server" /></p>
<p><strong>Süre:</strong> <asp:Label ID="lblPreviewTime" runat="server" /> dakika</p>
<p style="display:none;"><strong>Tip:</strong> <asp:Label ID="lblPreviewType" runat="server" /></p>
<p><strong>Durum:</strong> <asp:Label ID="lblPreviewStatus" runat="server" /></p>
                </div>
                
                <div class="col-md-4 text-center">
                    <asp:Image ID="imgPreviewCover" runat="server" CssClass="img-fluid rounded" 
                        style="max-height: 200px;" />
                </div>
            </div>
        </div>

        <!-- Sonuçlar -->
        <div class="soft-card mb-4">
            <h5 class="mb-3" style="color: var(--text-dark); font-weight: 600;">
                Olası Sonuçlar 
                (<asp:Label ID="Label1" runat="server" /> adet)
            </h5>
            
            <asp:GridView ID="gvPreviewResults" runat="server" CssClass="table table-sm" 
                AutoGenerateColumns="False" EmptyDataText="Sonuç yok">
                <Columns>
                    <asp:BoundField DataField="Title" HeaderText="Başlık" />
                    <asp:BoundField DataField="Tag" HeaderText="Etiket" />
                    <asp:BoundField DataField="Description" HeaderText="Açıklama" />
                </Columns>
            </asp:GridView>
        </div>

        <!-- Sorular -->
        <div class="soft-card">
            <h5 class="mb-3" style="color: var(--text-dark); font-weight: 600;">
                Sorular ve Seçenekler
                (<asp:Label ID="Label2" runat="server" /> soru)
            </h5>
            
            <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="rptPreviewQuestions_ItemDataBound">
                <ItemTemplate>
                    <div class="soft-card mb-3" style="background: var(--bg-cream);">
                        <h6><strong>Soru <%# Eval("OrderNo") %>:</strong> <%# Eval("Text") %></h6>
                        
                        <asp:Repeater ID="rptPreviewOptions" runat="server">
                            <ItemTemplate>
                                <div class="ms-3 mb-2">
                                    <strong><%# Eval("OrderNo") %>.</strong> <%# Eval("Text") %>
                                    <small class="text-muted">
                                        (Puanlar: <%# GetScoreSummary(Eval("PersonalityScores").ToString()) %>)
                                    </small>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div class="navigation-buttons">
            <button type="button" class="btn btn-secondary btn-lg" onclick="goToStep(3)">
                <i class="fas fa-arrow-left"></i> Geri
            </button>
            <asp:Button ID="Button1" runat="server" Text="Yayınla" 
                CssClass="btn btn-success btn-lg" OnClick="btnPublishQuiz_Click" />
            <asp:Button ID="Button2" runat="server" Text="Taslak Olarak Kaydet" 
                CssClass="btn btn-warning btn-lg" OnClick="btnSaveDraft_Click" />
        </div>
    </div>
</div>

            <!-- Sonuçlar -->
            <div class="soft-card section-green mb-4">
                <h5 class="mb-3" style="color: var(--text-dark); font-weight: 600;">
                    <i class="fas fa-trophy text-success"></i> Olası Sonuçlar 
                    <span class="badge bg-success">
                        <asp:Label ID="lblPreviewResultCount" runat="server" />
                    </span>
                </h5>
                
                <div class="row">
                    <asp:Repeater ID="rptPreviewResults" runat="server">
    <ItemTemplate>
        <div class="col-md-4 mb-3">
            <div class="result-preview-card">
                <!-- Görsel varsa göster, yoksa emoji göster -->
                <%# GetPreviewResultIcon(Eval("ImageUrl"), Eval("IconEmoji")) %>
                
                <h6><%# Eval("Title") %></h6>
                <span class="result-tag">#<%# Eval("Tag") %></span>
                <p class="result-desc"><%# Eval("Description") %></p>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

                </div>
            </div>

            <!-- Sorular ve Seçenekler -->
            <div class="soft-card section-purple">
                <h5 class="mb-4" style="color: var(--text-dark); font-weight: 600;">
                    <i class="fas fa-list-check text-primary"></i> Sorular ve Seçenekler
                    <span class="badge bg-primary">
                        <asp:Label ID="lblPreviewQuestionCount" runat="server" />
                    </span>
                </h5>
                
                <asp:Repeater ID="rptPreviewQuestions" runat="server" OnItemDataBound="rptPreviewQuestions_ItemDataBound">
                    <ItemTemplate>
                        <div class="question-preview-card mb-4">
                            <div class="question-header">
                                <span class="question-number">Soru <%# Eval("OrderNo") %></span>
                                <h6 class="question-text"><%# Eval("Text") %></h6>
                            </div>
                            
                            <div class="options-grid">
                                <asp:Repeater ID="rptPreviewOptions" runat="server">
                                    <ItemTemplate>
                                        <div class="option-preview-card">
                                            <div class="option-number"><%# Eval("OrderNo") %></div>
                                            <div class="option-content">
                                                <%# !string.IsNullOrEmpty(Eval("ImageUrl").ToString()) ? 
                                                    "<img src='" + Eval("ImageUrl") + "' class='option-image' />" : "" %>
                                                <div class="option-text"><%# Eval("Text") %></div>
                                            </div>
                                            <div class="option-scores">
                                                <small class="text-muted">
                                                    <i class="fas fa-star"></i> 
                                                    <%# GetScoreSummary(Eval("PersonalityScores").ToString()) %>
                                                </small>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <div class="navigation-buttons">
                <button type="button" class="btn btn-secondary btn-lg" onclick="goToStep(3)">
                    <i class="fas fa-arrow-left"></i> Geri
                </button>
                <asp:Button ID="btnPublishQuiz" runat="server" Text="Yayınla" 
                    CssClass="btn btn-success btn-lg" OnClick="btnPublishQuiz_Click" />
                <asp:Button ID="btnSaveDraft" runat="server" Text="Taslak Olarak Kaydet" 
                    CssClass="btn btn-warning btn-lg" OnClick="btnSaveDraft_Click" />
            </div>
        </div>
    

    <!-- JavaScript -->
    <script type="text/javascript">
        var editor;

        // Sayfa yüklendiğinde
        window.addEventListener('load', function () {
            console.log('Sayfa yüklendi');
            
            var txtDesc = document.getElementById('<%=txtDescription.ClientID%>');
            console.log('Description TextBox:', txtDesc);
            console.log('Description Değeri:', txtDesc ? txtDesc.value : 'bulunamadı');
            
            // CKEditor'ü başlat
            if (typeof CKEDITOR !== 'undefined') {
                if (CKEDITOR.instances['<%=txtDescription.ClientID%>']) {
                    CKEDITOR.instances['<%=txtDescription.ClientID%>'].destroy(true);
                }
                
                editor = CKEDITOR.replace('<%=txtDescription.ClientID%>', {
                    height: 180,
                    toolbar: [
                        { name: 'basicstyles', items: ['Bold', 'Italic'] },
                        { name: 'paragraph', items: ['NumberedList', 'BulletedList'] }
                    ]
                });
                
                console.log('CKEditor başlatıldı');
            } else {
                console.error('CKEDITOR yüklenmedi!');
            }
        });


        function saveCKEditorData() {
            try {
                if (typeof editor !== 'undefined' && editor && editor.getData) {
                    var descBox = document.getElementById('<%=txtDescription.ClientID%>');
                    if (descBox) {
                        descBox.value = editor.getData();
                    }
                }
            } catch (e) {
                console.log('Form submit CKEditor hatası:', e);
            }
            return true; // Form submit devam etsin
        }

        function goToStep(step) {
            // CKEditor verilerini kaydet
            try {
                if (typeof editor !== 'undefined' && editor && editor.getData) {
                    var descBox = document.getElementById('<%=txtDescription.ClientID%>');
            if (descBox) {
                descBox.value = editor.getData();
            }
        }
    } catch (e) {
        console.log('CKEditor veri kaydetme hatası:', e);
    }

            document.getElementById('panel' + step).classList.add('active');
            document.querySelector('.step[data-step="' + step + '"]').classList.add('active');
            document.getElementById('<%=hfCurrentStep.ClientID%>').value = step;
            
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }

        document.addEventListener('DOMContentLoaded', function() {
            document.querySelectorAll('.step').forEach(step => {
                step.addEventListener('click', function () {
                    goToStep(this.getAttribute('data-step'));
                });
            });
        });

        function loadScoreInputs(results) {
            const container = document.getElementById('scoreInputsContainer');
            if (!container) return;
            
            container.innerHTML = '';
            
            results.forEach(result => {
                const div = document.createElement('div');
                div.className = 'score-item';
                div.innerHTML = `
                    <div class="d-flex justify-content-between align-items-center mb-2">
                        <strong style="color: var(--text-dark); font-size: 0.9rem;">${result.Title}</strong>
                        <span class="badge bg-secondary">#${result.Tag}</span>
                    </div>
                    <input type="number" 
                           id="score_${result.Tag}" 
                           name="score_${result.Tag}"
                           class="form-control" 
                           min="0" max="10" 
                           value="0" 
                           placeholder="0-10"
                           onchange="updateScoreData('${result.Tag}', this.value)" />
                `;
                container.appendChild(div);
            });
        }

        function updateScoreData(tag, value) {
            const hf = document.getElementById('<%=hfScoreData.ClientID%>');
            if (!hf) return;

            let scores = {};
            try {
                scores = JSON.parse(hf.value || '{}');
            } catch (e) {
                scores = {};
            }

            scores[tag] = parseInt(value) || 0;
            hf.value = JSON.stringify(scores);
        }


        // ==================== BİLGİLENDİRME PANELİ ====================
        function toggleInfoPanel() {
            const panel = document.getElementById('infoPanel');
            const btn = document.querySelector('.info-toggle-btn');

            if (panel.style.display === 'none') {
                panel.style.display = 'block';
                btn.classList.add('active');
            } else {
                panel.style.display = 'none';
                btn.classList.remove('active');
            }
        }


        // ==================== GÖRSEL ÖNİZLEME SİSTEMİ ====================

        // Dosya boyutu formatla
        function formatFileSize(bytes) {
            if (bytes === 0) return '0 Bytes';
            const k = 1024;
            const sizes = ['Bytes', 'KB', 'MB', 'GB'];
            const i = Math.floor(Math.log(bytes) / Math.log(k));
            return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
        }

        // Dosya validasyonu
        function validateImageFile(file, maxSizeMB = 5) {
            const validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
            const maxSize = maxSizeMB * 1024 * 1024;

            if (!validTypes.includes(file.type)) {
                return {
                    valid: false,
                    message: '❌ Geçersiz dosya tipi! İzin verilenler: JPG, PNG, GIF, WEBP'
                };
            }

            if (file.size > maxSize) {
                return {
                    valid: false,
                    message: `❌ Dosya çok büyük! Maksimum ${maxSizeMB}MB olmalıdır. (Mevcut: ${formatFileSize(file.size)})`
                };
            }

            return {
                valid: true,
                message: '✓ Dosya başarıyla seçildi'
            };
        }

        // Görsel önizleme oluştur
        function setupImagePreview(fileUploadId, previewWrapperId) {
            const fileUpload = document.getElementById(fileUploadId);
            const previewWrapper = document.getElementById(previewWrapperId);

            if (!fileUpload || !previewWrapper) {
                console.error('FileUpload veya Preview elementi bulunamadı:', fileUploadId, previewWrapperId);
                return;
            }

            fileUpload.addEventListener('change', function (e) {
                const file = e.target.files[0];

                if (!file) {
                    previewWrapper.style.display = 'none';
                    return;
                }

                // Dosya validasyonu
                const validation = validateImageFile(file);
                const messageDiv = previewWrapper.querySelector('.file-validation-message');

                if (!validation.valid) {
                    messageDiv.innerHTML = `<i class="fas fa-exclamation-circle"></i> ${validation.message}`;
                    messageDiv.className = 'file-validation-message error';
                    messageDiv.style.display = 'block';

                    previewWrapper.querySelector('.image-preview-box').classList.remove('has-image');
                    previewWrapper.querySelector('.preview-image-container').innerHTML = '';
                    previewWrapper.querySelector('.image-info').innerHTML = '';

                    // Dosyayı temizle
                    fileUpload.value = '';
                    return;
                }

                // Başarılı mesaj
                messageDiv.innerHTML = `<i class="fas fa-check-circle"></i> ${validation.message}`;
                messageDiv.className = 'file-validation-message success';
                messageDiv.style.display = 'block';

                // Önizleme göster
                const reader = new FileReader();
                reader.onload = function (event) {
                    const previewBox = previewWrapper.querySelector('.image-preview-box');
                    const imageContainer = previewWrapper.querySelector('.preview-image-container');
                    const imageInfo = previewWrapper.querySelector('.image-info');

                    previewBox.classList.add('has-image');

                    imageContainer.innerHTML = `
                <img src="${event.target.result}" class="preview-image" alt="Önizleme" />
            `;

                    // Dosya bilgileri
                    imageInfo.innerHTML = `
                <div class="image-info-item">
                    <i class="fas fa-file"></i>
                    <span>${file.name}</span>
                </div>
                <div class="image-info-item">
                    <i class="fas fa-weight"></i>
                    <span>${formatFileSize(file.size)}</span>
                </div>
                <div class="image-info-item">
                    <i class="fas fa-image"></i>
                    <span>${file.type.split('/')[1].toUpperCase()}</span>
                </div>
            `;

                    previewWrapper.style.display = 'block';
                };

                reader.readAsDataURL(file);
            });

            // Görseli kaldır butonu
            const removeBtn = previewWrapper.querySelector('.btn-remove-image');
            if (removeBtn) {
                removeBtn.addEventListener('click', function () {
                    fileUpload.value = '';
                    previewWrapper.style.display = 'none';
                    previewWrapper.querySelector('.image-preview-box').classList.remove('has-image');
                    previewWrapper.querySelector('.preview-image-container').innerHTML = '';
                    previewWrapper.querySelector('.image-info').innerHTML = '';
                    previewWrapper.querySelector('.file-validation-message').style.display = 'none';
                });
            }

            // Değiştir butonu
            const changeBtn = previewWrapper.querySelector('.btn-change-image');
            if (changeBtn) {
                changeBtn.addEventListener('click', function () {
                    fileUpload.click();
                });
            }
        }

        // Sayfa yüklendiğinde tüm upload kontrollerini ayarla
        document.addEventListener('DOMContentLoaded', function () {
            // Quiz Cover Image
            setupImagePreview('<%=fuCoverImage.ClientID%>', 'previewCoverImage');

    // Result Image
    setupImagePreview('<%=fuResultImage.ClientID%>', 'previewResultImage');
    
    // Question Image
    setupImagePreview('<%=fuQuestionImage.ClientID%>', 'previewQuestionImage');
    
    // Option Image
    setupImagePreview('<%=fuOptionImage.ClientID%>', 'previewOptionImage');
        });

        // ==================== MEVCUT GÖRSELİ GÖSTER ====================
        function showExistingImage(previewWrapperId, imageUrl, fileName) {
            const previewWrapper = document.getElementById(previewWrapperId);

            if (!previewWrapper || !imageUrl) return;

            const previewBox = previewWrapper.querySelector('.image-preview-box');
            const imageContainer = previewWrapper.querySelector('.preview-image-container');
            const imageInfo = previewWrapper.querySelector('.image-info');
            const messageDiv = previewWrapper.querySelector('.file-validation-message');

            previewBox.classList.add('has-image');

            imageContainer.innerHTML = `
        <img src="${imageUrl}" class="preview-image" alt="Mevcut Görsel" />
    `;

            imageInfo.innerHTML = `
        <div class="image-info-item">
            <i class="fas fa-check-circle"></i>
            <span style="color: var(--soft-green); font-weight: 600;">Mevcut Görsel</span>
        </div>
        <div class="image-info-item">
            <i class="fas fa-file"></i>
            <span>${fileName || 'Görsel'}</span>
        </div>
    `;

            messageDiv.innerHTML = `<i class="fas fa-info-circle"></i> Değiştirmek için yeni dosya seçin`;
            messageDiv.className = 'file-validation-message success';
            messageDiv.style.display = 'block';

            previewWrapper.style.display = 'block';
        }
    </script>
</asp:Content>