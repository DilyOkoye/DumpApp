function OnBegin() {
    console.log("On Begin Function");
}

function OnComplete() {
    console.log("On Complete Function");

}
function OnSuccess(data) {
   // alert("Test")
    //alert(data.Object.rv.sErrorText)
    //alert(data.Object.rv.nErrorCode)
    if (data.Success) {
        //  alert(data.Object.rv.nErrorCode)
        // alert(data.Object.rv.sErrorText)
        if (data.Object.rv.nErrorCode === 0) {
            console.log(data.Object.rv.sErrorText);
            swal("Success!", data.Object.rv.sErrorText, "success");
           // alert("test")
            if (data.Object.Url !== null) {
                window.location.href = data.Object.Url
            }
            
        }

        else {

            console.log(data.Object.rv.sErrorText);
            swal("Error!", data.Object.rv.sErrorText, "error");

        }
    }
    else {
        swal("Error!", data.Object.rv.sErrorText, "error");
    }

}

function OnFailure() {
    swal("Error!", data.Object.rv.sErrorText, "error");

}