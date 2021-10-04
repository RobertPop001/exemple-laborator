using System;
using Lucrarea01.Domain;
using System.Collections.Generic;
using static Lucrarea01.Domain.Cos;

namespace Lucrarea01
{
    class Program
    {

        static void Main(string[] args)
        {

            var answer = ReadValue("Incepeti cumparaturile?[Y/N]: ");
            if (answer.ToLower().Contains("y"))
            {
                var listOfProduse = ReadProduse().ToArray();
                var cosDetails = ReadDetails();

                UnvalidatedCos unvalidatedCos = new(listOfProduse, cosDetails);

                ICos result = CheckCos(unvalidatedCos);
                result.Match(
                    whenUnvalidatedCos: unvalidatedCos => unvalidatedCos,
                    whenGolCos: invalidResult => invalidResult,
                    whenInvalidatedCos: invalidResult => invalidResult,
                    whenValidatedCos: validatedCos => CosPlatit(validatedCos, cosDetails,DateTime.Now),
                    whenCosPlatit: cosPlatit => cosPlatit
                );

                Console.WriteLine(result);

            }
            else Console.WriteLine("BYE!");

        }
        private static ICos CheckCos(UnvalidatedCos unvalidatedCos) =>
           ( (unvalidatedCos.ProduseList.Count == 0) ? new GolCos(new List<UnvalidatedProduse>(), "cos gol")
                : ((string.IsNullOrEmpty(unvalidatedCos.CosDetails.PaymentAddress.Value))? new InvalidatedCos(new List<UnvalidatedProduse>(), "Cos Invalid")
                      :( (unvalidatedCos.CosDetails.PaymentState.Value == 0) ? new ValidatedCos(new List<ValidatedProduse>(), unvalidatedCos.CosDetails)
                             :new CosPlatit(new List<ValidatedProduse>(), unvalidatedCos.CosDetails, DateTime.Now))));
        
        private static ICos CosPlatit(ValidatedCos validatedResult, CosDetails cosDetails, DateTime PublishedDate) =>
                new CosPlatit(new List<ValidatedProduse>(), cosDetails, DateTime.Now);

        private static List<UnvalidatedProduse> ReadProduse()
        {
            List<UnvalidatedProduse> listOfProduse = new();
            var answer = string.Empty;
            do
            {
                answer = ReadValue("Adaugati produs?[Y/N]: ");

                if (answer.ToLower().Equals("y"))
                {
                    var ProdusID = ReadValue("Produs ID: ");
                    if (string.IsNullOrEmpty(ProdusID))
                    {
                        break;
                    }

                    var ProdusCantitate = ReadValue("Cantitate produs: ");
                    if (string.IsNullOrEmpty(ProdusCantitate))
                    {
                        break;
                    }
                    UnvalidatedProduse toAdd = new(ProdusID, ProdusCantitate);
                    listOfProduse.Add(toAdd);
                }

            } while (answer.ToLower().Equals("y"));
            
            return listOfProduse;
        }

        public static CosDetails ReadDetails()
        {
            PaymentState paymentState;
            PaymentAddress paymentAddress;
            CosDetails cosDetails;

            string answer = ReadValue("Finalizezi comanda?[Y/N]: ");

            if (answer.ToLower().Equals("y"))
            {

                var Address = ReadValue("Adresa: ");
                if (string.IsNullOrEmpty(Address))
                {
                    paymentAddress = new PaymentAddress("NONE");
                }
                else
                {
                    paymentAddress = new PaymentAddress(Address);
                }
                var payment = ReadValue("Platesti?[Y/N]: ");
                if (payment.ToLower().Equals("y"))
                {
                    paymentState = new PaymentState(1);
                }
                else
                {
                    paymentState = new PaymentState(0);
                }
            }
            else
            {
                paymentAddress = new PaymentAddress("NONE");
                paymentState = new PaymentState(0);
            }
            cosDetails = new CosDetails(paymentAddress, paymentState);
            return cosDetails;
         }

        private static string ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

    }
}
