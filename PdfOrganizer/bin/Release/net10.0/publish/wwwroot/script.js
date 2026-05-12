async function uploadPDF() {

    const status = document.getElementById("status");

    const fileInput = document.getElementById("pdfFile");

    if (!fileInput.files.length) {
        status.innerText = "Selecione um PDF";
        return;
    }

    status.innerText = "Processando PDF...";

    const formData = new FormData();

    formData.append("file", fileInput.files[0]);

    try {

        const response = await fetch("/upload", {
            method: "POST",
            body: formData
        });

        if (!response.ok) {
            throw new Error("Erro no processamento");
        }

        const blob = await response.blob();

        const url = window.URL.createObjectURL(blob);

        const a = document.createElement("a");

        a.href = url;
        a.download = "resultado.pdf";

        document.body.appendChild(a);

        a.click();

        a.remove();

        status.innerText = "PDF organizado com sucesso";

    } catch (error) {

        status.innerText = "Erro ao processar PDF";

        console.error(error);
    }
}