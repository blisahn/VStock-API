using Microsoft.AspNetCore.Identity;

namespace VBorsa_API.Application.Exceptions;

public static class ExceptionHelper
{
    public static string GetCustomErrorMessage(IEnumerable<IdentityError> errors)
    {
        foreach (var error in errors)
            return error.Code switch
            {
                "DuplicateUserName" => "Bu kullanıcı adı zaten alınmış. Lütfen başka bir kullanıcı adı seçin.",
                "DuplicateEmail" => "Bu e-posta adresi zaten kullanılmakta. Lütfen başka bir e-posta adresi girin.",
                "InvalidEmail" => "Geçersiz e-posta adresi formatı.",
                _ => "Kayıt olurken bir hata oluştu. Lütfen bilgilerinizi kontrol edin."
            };
        return "Kayıt olurken bilinmeyen bir hata oluştu.";
    }
}