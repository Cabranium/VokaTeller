
//Um den Download der Datenbank auszulösen
function downloadFile(filename, content) {
    var blob = new Blob([content], { type: 'application/json' });
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = filename;
    link.click();
    link.remove();

}