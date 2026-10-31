

using System.Globalization;

namespace Hesap_makinesi.Ekranlar;


public partial class Bilimsel : ContentPage
{
    private static readonly CultureInfo TR = new("tr-TR");   

    private double _ilk;              
    private string _islem = "";        
    private bool _yeniGiris = true;    
    private bool _hata;                

    public Bilimsel()
    {
        InitializeComponent();
    }

    
    private void OnKeyClicked(object? sender, EventArgs e)
    {
        string tus = (string)((Button)sender!).CommandParameter;

        if (_hata) Temizle();  

        if (char.IsDigit(tus[0])) { Rakam(tus); return; }

        switch (tus)
        {
            case ".": Virgul(); break;
            case "+": case "-": case "*": case "/": case "^": IslemSec(tus); break;
            case "=": Esittir(); break;
            case "C": Temizle(); break;
            case "BS": Sil(); break;
            case "NEG": Isaret(); break;
            case "SQRT": case "LOG": case "LN": case "SIN": case "COS": case "TAN": case "SEC": case "CSC": case "COT":
                Fonksiyon(tus); break;
        }
    }


    // sayı girer
    private void Rakam(string rakam)
    {
        if (_yeniGiris || ResultLabel.Text == "0") ResultLabel.Text = rakam;
        else if (ResultLabel.Text.Length < 15) ResultLabel.Text += rakam;
        _yeniGiris = false;
    }

    private void Virgul()
    {
        if (_yeniGiris) { ResultLabel.Text = "0,"; _yeniGiris = false; }
        else if (!ResultLabel.Text.Contains(',')) ResultLabel.Text += ",";
    }

    private void Sil()
    {
        if (_yeniGiris) return;
        string metin = ResultLabel.Text[..^1];
        ResultLabel.Text = metin is "" or "-" ? "0" : metin;
    }

    private void Isaret()
    {
        if (ResultLabel.Text == "0") return;
        ResultLabel.Text = ResultLabel.Text.StartsWith('-') ? ResultLabel.Text[1..] : "-" + ResultLabel.Text;
    }

    // yapılacak işlemin belirlenmesi
    private void IslemSec(string islem)
    {
        if (_islem != "" && !_yeniGiris && !Hesapla(Sayi())) return;

        _ilk = Sayi();
        _islem = islem;
        _yeniGiris = true;
        ExpressionLabel.Text = $"{Yaz(_ilk)} {Sembol(islem)}";
    }

    private void Esittir()
    {
        if (_islem == "") return;

        double ikinci = Sayi();
        string ifade = $"{Yaz(_ilk)} {Sembol(_islem)} {Yaz(ikinci)} =";
        if (!Hesapla(ikinci)) return;

        ExpressionLabel.Text = ifade;
        _islem = "";
        _yeniGiris = true;
    }

    //yapılacak işlemin hesaplanması
    private bool Hesapla(double ikinci)
    {
        double sonuc = 0;
        switch (_islem)
        {
            case "+": sonuc = _ilk + ikinci; break;
            case "-": sonuc = _ilk - ikinci; break;
            case "*": sonuc = _ilk * ikinci; break;
            case "/":
                if (ikinci == 0) { Hata("Sıfıra bölünemez"); return false; }
                sonuc = _ilk / ikinci;
                break;
            case "^": sonuc = Math.Pow(_ilk, ikinci); break;
        }

        if (double.IsNaN(sonuc) || double.IsInfinity(sonuc)) { Hata("Geçersiz işlem"); return false; }

        ResultLabel.Text = Yaz(sonuc);
        return true;
    }


    // bundan önceki kısım standarta ek üs alma işlemini içeriyor
    // burada bilimsel için gerekli karekök, logaritma ve trigonometri işlemleri mevcut
    private void Fonksiyon(string tus)
    {
        double x = Sayi();
        double radyan = x * Math.PI / 180;   
        double sin = Math.Sin(radyan), cos = Math.Cos(radyan);
        double sonuc;
        string ad;

        switch (tus)
        {
            case "SQRT":
                if (x < 0) { Hata("Geçersiz giriş"); return; }
                sonuc = Math.Sqrt(x); ad = "√";
                break;
            case "LOG":
                if (x <= 0) { Hata("Geçersiz giriş"); return; }
                sonuc = Math.Log10(x); ad = "log";
                break;
            case "LN":
                if (x <= 0) { Hata("Geçersiz giriş"); return; }
                sonuc = Math.Log(x); ad = "ln";
                break;
            case "SIN": sonuc = Math.Round(sin, 12); ad = "sin"; break;   
            case "COS": sonuc = Math.Round(cos, 12); ad = "cos"; break;
            case "TAN":
                if (Math.Abs(cos) < 1e-12) { Hata("Tanımsız"); return; }   
                sonuc = Math.Round(sin / cos, 12); ad = "tan";
                break;
            case "SEC":   
                if (Math.Abs(cos) < 1e-12) { Hata("Tanımsız"); return; }
                sonuc = Math.Round(1 / cos, 12); ad = "sec";
                break;
            case "CSC":   
                if (Math.Abs(sin) < 1e-12) { Hata("Tanımsız"); return; }
                sonuc = Math.Round(1 / sin, 12); ad = "csc";
                break;
            default:      
                if (Math.Abs(sin) < 1e-12) { Hata("Tanımsız"); return; }
                sonuc = Math.Round(cos / sin, 12); ad = "cot";
                break;
        }

        
        string onek = _islem == "" ? "" : $"{Yaz(_ilk)} {Sembol(_islem)} ";
        ExpressionLabel.Text = $"{onek}{ad}({Yaz(x)})";
        ResultLabel.Text = Yaz(sonuc);
        _yeniGiris = true;
    }

    
    private double Sayi() => double.Parse(ResultLabel.Text, NumberStyles.Float, TR);

    private static string Yaz(double sayi) => sayi == 0 ? "0" : sayi.ToString("G15", TR);

    private static string Sembol(string islem) => islem switch { "*" => "×", "/" => "÷", "-" => "−", _ => islem };

    private void Temizle()
    {
        _ilk = 0; _islem = ""; _yeniGiris = true; _hata = false;
        ResultLabel.Text = "0";
        ExpressionLabel.Text = "";
    }

    // hatayı ekrana verir
    private void Hata(string mesaj)
    {
        Temizle();
        ResultLabel.Text = mesaj;
        _hata = true;
        DisplayAlert("Hata", mesaj, "Tamam");
    }
}
