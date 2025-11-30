using UserService.Domain.Interfaces;

namespace UserService.Infrastructure.Templates
{
    public  class EmailTemplates : IEmailTemplates
    {
        public EmailTemplates()
        { }
        public string CreateConfirmationEmail(string code, string link)
        {
            return $$"""
            <!DOCTYPE html>
            <html>
            <body style="text-align: center; padding: 40px; font-family: Arial;">
                <h2>InnoShop</h2>
                <p>Код подтверждения почты:</p>
                <div style="font-size: 32px; font-weight: bold; color: #2c5aa0;">
                    {{code}}
                </div>
                <p>Или перейдите по ссылке: <a href={{link}}>Подтвердить почту</a></p>
                <p>Сообщение действительно в течении 10 минут.</p>
            </body>
            </html>
            """;
        }

        public string CreateResetPasswordEmail(string code)
        {
            return $$"""
            <!DOCTYPE html>
            <html>
            <body style="text-align: center; padding: 40px; font-family: Arial;">
                <h2>InnoShop</h2>
                <p>Код сброса пароля:</p>
                <div style="font-size: 32px; font-weight: bold; color: #2c5aa0;">
                    {{code}}
                </div>
              <p>Сообщение действительно в течении 30 минут.</p>
            </body>
            </html>
            """;
        }
    }
}
