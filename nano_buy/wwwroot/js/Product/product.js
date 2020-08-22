$(document).ready(function () {
    $("form#product #create-product").on("click", async function (e) {
        e.preventDefault();
        var model = {};
        var productPrice = $("form#product #product-price").val();
        var productName = $("form#product #product-name").val();
        if (!productPrice) {
            alert("Please enter product price");
            return;
        }
        if (!productName) {
            alert("Please enter product name");
            return;
        }
        model["Price"] = parseFloat(productPrice);
        model["Name"] = productName;
        var filesToUpload = $("form#product #product-image").get(0).files;
        if (filesToUpload.length == 0) {
            alert("Please upload product image");
            return;
        }
        model["Base64Image"] = await ConvertDocumentToBase64(filesToUpload[0]);
        model["Base64ImageContentType"] = filesToUpload[0]["type"];
        var stringifiedModel = JSON.stringify(model);

        $.ajax({
            contentType: "application/json",
            method: NanoShop.POST,
            url: NanoShop.BasePath + "product/create",
            data: stringifiedModel,
            error: function (error) {
                console.log(error);
            },
            success: function (response) {
                alert(response);
            }
        });
    });
});
