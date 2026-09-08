namespace Byd.FileManagement.Options;

/// <summary>
/// Modulu tuketen host'un PreConfigure ile ezebilecegi calisma zamani secenekleri.
/// </summary>
// islevi: Modulun kendi depolama kablolamasini kurup kurmayacagini ve hangi bilgilerle kuracagini tasir.
// sistemdeki gorevi: Modul makul varsayilanlarla gelir; baska bir depolama saglayicisi takmak
// isteyen host varsayilan kablolamayi kapatir ve kendi container'ini yapilandirir.
public class FileManagementModuleOptions
{
    // Modulun kendi MinIO kablolamasini kurup kurmayacagi. Kendi saglayicisini takan host kapatir.
    public bool IsDefaultStorageEnabled { get; set; }

    // Kablolamada kullanilacak depolama bilgileri; hazir nesneyle gelir, dolduran taraf alanlarini yazar.
    public FileStorageOptions FileStorage { get; set; } = new();
}
