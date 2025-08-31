using System.Collections.Specialized;
using Prometheus;

namespace VBorsa_API.Presentation.Monitoring;

public static class AppMetrics
{
    public static readonly Counter RegisteredUsersCounter = Metrics.CreateCounter(
        "vborsa_users_registered_total",
        "Uygulamaya kayıt olan toplam kullanıcı sayısı." 
    );
    public static readonly Counter CreatedAssets = Metrics.CreateCounter(
        "vborsa_assets_registered_total",
        "Uygulamayada kayit olan toplam degerli varlik sayisi." 
    );
    
    public static readonly Counter TotalBuy = Metrics.CreateCounter(
        "vborsa_assets_total_buy",
        "Uygulamayada kayit olan toplam degerli varlik sayisi." 
    );
    public static readonly Counter TotalSales = Metrics.CreateCounter(
        "vborsa_assets_total_sales",
        "Uygulamayada kayit olan toplam degerli varlik sayisi." 
    );
}