using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PdfOrganizer.Services;

public class PdfService
{
    public string ProcessPdf(string inputPath)
    {
        var linhas = new List<string>();

        using (PdfDocument document = PdfDocument.Open(inputPath))
        {
            foreach (var page in document.GetPages())
            {
                // Extract lines by grouping words with similar Y positions
                var words = page.GetWords()
                    .GroupBy(w => Math.Round(w.BoundingBox.Bottom, 0))
                    .OrderByDescending(g => g.Key);

                foreach (var lineGroup in words)
                {
                    var linha = string.Join(" ", lineGroup
                        .OrderBy(w => w.BoundingBox.Left)
                        .Select(w => w.Text))
                        .Trim();

                    if (string.IsNullOrWhiteSpace(linha)) continue;
                    if (linha.Contains("Impresso em")) continue;
                    if (linha.Contains("Página")) continue;
                    if (linha.Contains("Não Atendidos")) continue;

                    bool produtoValido = Regex.IsMatch(linha, @"^\d+\.\d+");

                    if (produtoValido)
                        linhas.Add(linha);
                }
            }
        }

        var ordenado = linhas
            .OrderBy(l => {
                var partes = l.Split(' ', 2);

                return partes.Length > 1
                    ? partes[1]
                    : l;
            })
            .ToList();

        var outputPath = Path.Combine(
            Path.GetTempPath(),
            $"resultado_{Guid.NewGuid()}.pdf"
        );

        Document.Create(container => {
            container.Page(page => {
                page.Margin(30);

                page.Header()
                    .Text("PDF Organizado")
                    .FontSize(20)
                    .Bold();

                page.Content()
                    .Column(col => {
                        foreach (var item in ordenado)
                        {
                            col.Item().Text(item);
                        }
                    });
            });
        })
        .GeneratePdf(outputPath);

        return outputPath;
    }
}