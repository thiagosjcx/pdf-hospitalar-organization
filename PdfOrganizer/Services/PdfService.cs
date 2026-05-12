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
                var texto = page.Text;

                var split = texto.Split('\n');

                foreach (var linha in split)
                {
                    var limpa = linha.Trim();

                    if (string.IsNullOrWhiteSpace(limpa))
                        continue;

                    if (limpa.Contains("Impresso em"))
                        continue;

                    if (limpa.Contains("Página"))
                        continue;

                    if (limpa.Contains("Não Atendidos"))
                        continue;

                    bool produtoValido = Regex.IsMatch(
                        limpa,
                        @"^\d+\.\d+"
                    );

                    if (produtoValido)
                    {
                        linhas.Add(limpa);
                    }
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