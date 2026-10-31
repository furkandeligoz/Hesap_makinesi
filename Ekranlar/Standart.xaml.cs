

using System.Globalization;

namespace Hesap_makinesi.Ekranlar;


public partial class Standart : ContentPage
{
    private static readonly CultureInfo TR = new("tr-TR");  

    private double _ilk;              
    private string _islem = "";        
    private bool _yeniGiris = true;    
    private bool _hata;                

    public Standart()
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
            case "+": case "-": case "*": case "/": IslemSec(tus); break;
            case "=": Esittir(); break;
            case "C": Temizle(); break;
            case "CE": ResultLabel.Text = "0"; _yeniGiris = true; break;
            case "BS": Sil(); break;
            case "NEG": Isaret(); break;
        }
    }

    
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
        }

        if (double.IsNaN(sonuc) || double.IsInfinity(sonuc)) { Hata("Geçersiz işlem"); return false; }

        ResultLabel.Text = Yaz(sonuc);
        return true;
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

    
    private void Hata(string mesaj)
    {
        Temizle();
        ResultLabel.Text = mesaj;
        _hata = true;
        DisplayAlert("Hata", mesaj, "Tamam");
    }
}
