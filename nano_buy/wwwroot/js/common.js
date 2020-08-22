var NanoShop = {
    "GET": "GET",
    "PUT": "PUT",
    "POST": "POST",
    "DELETE": "DELETE",
    "BasePath": window.location.origin + "/",
}

const ConvertDocumentToBase64 = file => new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsBinaryString(file);
    reader.onload = () => resolve(window.btoa(reader.result));
    reader.onerror = error => reject(error);
});
