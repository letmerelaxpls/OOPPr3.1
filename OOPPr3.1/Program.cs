using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOPPr3._1
{
    internal class Transaction
    {
        public DateTime DateTime { get; set; }
        public double Amount { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            {
                string csvFilePath = "transactions.csv";
                string dateFormat = "yyyy-MM-dd";
                int batchSize = 10;

                Func<string, DateTime> getDate = (line) =>
                {
                    string[] fields = line.Split(',');
                    return DateTime.ParseExact(fields[0], dateFormat, null);
                };

                Func<string, double> getAmount = (line) =>
                {
                    string[] fields = line.Split(',');
                    return double.Parse(fields[1]);
                };

                Action<DateTime, double> printTotalAmount = (date, totalAmount) =>
                {
                    Console.WriteLine($"{date.ToString(dateFormat)}: {totalAmount}");
                };

                int page = 1;
                List<string> lines = File.ReadAllLines(csvFilePath).ToList();
                List<Transaction> transactions = new List<Transaction>();
                for (int i = 0; i < lines.Count; i++)
                {
                    DateTime currentDay = getDate(lines[i]);
                    Transaction transaction = new Transaction();
                    if (!transactions.Any(tr => tr.DateTime == currentDay))
                        transactions.Add(new Transaction() { DateTime = currentDay });
                    transaction = transactions.FirstOrDefault(tr => tr.DateTime == currentDay);

                    double amount = getAmount(lines[i]);
                    if (transaction.DateTime == currentDay)
                        transaction.Amount += amount;

                    if (i % 10 == 0 && i != 0)
                    {
                        WriteCsv(csvFilePath, $"transactions{page}.csv", dateFormat, i - 10);
                        page++;
                    }
                }
                if (lines.Count - 1 % 10 != 0)
                    WriteCsv(csvFilePath, $"transactions{page}.csv", dateFormat, (page - 1) * 10);
                foreach (var transaction in transactions)
                    printTotalAmount(transaction.DateTime, transaction.Amount);
            }
              void WriteCsv(string dataPath, string resultFilePath, string dateFormat, int start)
            {
                var lines = File.ReadAllLines(dataPath);
                int end = start + 10 > lines.Length - 1 ? lines.Length - 1 : start + 10;
                double currentTotal = 0;
                List<string> result = new List<string>();

                for (int i = start; i < end; i++)
                    result.Add(lines[i]);

                File.WriteAllLines(resultFilePath, result);
            }
        }
    }
}
