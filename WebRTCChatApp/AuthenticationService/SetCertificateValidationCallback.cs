using System.Net.Security;

public class CertificateValidator
{
//     public static RemoteCertificateValidationCallback GetServerCertificateValidationCallback()
//     {
//         return (sender, cert, chain, sslPolicyErrors) =>
//         {
//             Console.WriteLine($"Certificate Validation Callback called with errors: {sslPolicyErrors}");

//             foreach (SslPolicyErrors error in Enum.GetValues(typeof(SslPolicyErrors)))
//             {
//                 if ((error & sslPolicyErrors) != 0)
//                 {
//                     Console.WriteLine($"SSL Policy Error: {error}");
//                 }
//             }

//             return true;
//         };
//     }

    public static RemoteCertificateValidationCallback GetServerCertificateValidationCallback()
    {
        return (sender, cert, chain, sslPolicyErrors) => 
        {
            // Always return true to accept the certificate
            return true;
        };
    }
}