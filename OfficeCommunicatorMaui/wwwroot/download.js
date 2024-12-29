window.blazorDownloadFile = (fileName, contentType, base64Data) => {
    const link = document.createElement("a");
    link.download = fileName;
    link.href = `data:${contentType};base64,${base64Data}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};


window.scrollToBottom = (container) => {
    if (container) {
        container.scrollTop = container.scrollHeight;
    }
};