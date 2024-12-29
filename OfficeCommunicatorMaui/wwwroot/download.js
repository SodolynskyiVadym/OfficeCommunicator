window.blazorDownloadFile = (fileName, contentType, base64Data) => {
    const link = document.createElement("a");
    link.download = fileName;
    link.href = `data:${contentType};base64,${base64Data}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};


window.printData = (documentId, messageId, chatId) => {
    console.log(`Document ${documentId}, message Id ${messageId}, chat id ${chatId}`);
};