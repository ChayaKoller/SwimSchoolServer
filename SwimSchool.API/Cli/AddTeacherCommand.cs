using System.ComponentModel.DataAnnotations;
using System.Text;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Teachers;
using SwimSchool.Application.Teachers.Dtos;

namespace SwimSchool.API.Cli;

// English prompts only: the Windows console shows Hebrew reversed
public static class AddTeacherCommand
{
    public static async Task RunAsync(IServiceProvider services)
    {
        var identityNumber = Read("Identity number (9 digits)");
        var fullName = Read("Full name");
        var email = Read("Email");
        var phone = Read("Phone");
        var password = ReadPasswordTwice();

        var request = new CreateTeacherRequest
        {
            IdentityNumber = identityNumber,
            FullName = fullName,
            Email = email,
            Phone = phone,
            Password = password
        };

        var errors = new List<ValidationResult>();
        if (!Validator.TryValidateObject(request, new ValidationContext(request), errors, validateAllProperties: true))
        {
            foreach (var error in errors)
                Console.WriteLine($"- {string.Join(", ", error.MemberNames)}: invalid");

            Environment.ExitCode = 1;
            return;
        }

        using var scope = services.CreateScope();
        var accounts = scope.ServiceProvider.GetRequiredService<ITeacherAccountService>();

        try
        {
            var id = await accounts.CreateTeacherAsync(request);
            Console.WriteLine($"Teacher added. Id: {id}");
        }
        catch (ConflictException ex)
        {
            Console.WriteLine($"Not saved: {ex.Code}");
            Environment.ExitCode = 1;
        }
    }

    private static string Read(string prompt)
    {
        Console.Write($"{prompt}: ");
        return Console.ReadLine() ?? string.Empty;
    }

    private static string ReadPasswordTwice()
    {
        while (true)
        {
            var password = ReadHidden("Password");
            var confirm = ReadHidden("Repeat password");

            if (password == confirm)
                return password;

            Console.WriteLine("The passwords do not match. Try again.");
        }
    }

    // Nothing is shown on screen
    private static string ReadHidden(string prompt)
    {
        Console.Write($"{prompt}: ");
        var value = new StringBuilder();

        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
                break;

            if (key.Key == ConsoleKey.Backspace)
            {
                if (value.Length > 0)
                    value.Length--;
            }
            else if (!char.IsControl(key.KeyChar))
            {
                value.Append(key.KeyChar);
            }
        }

        Console.WriteLine();
        return value.ToString();
    }
}
