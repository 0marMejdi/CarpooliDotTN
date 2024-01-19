// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


function popFlash(message,color){
    let flashAlert = makeFlash(message,color);
    document.body.appendChild(flashAlert);
    setTimeout(()=>{
        if (flashAlert !== null){
            console.log(flashAlert);
            //    flashAlert.style.opacity="0";
            flashAlert.classList.add("fade")
            flashAlert.style.opacity="0";
            flashAlert.style.cssText=" opacity: 0 ; ";
            flashAlert.remove();
        }
    },2000);
}
function popFlashError(message){
    popFlash(message,'danger');
}

function popFlashSuccess(message){
    popFlash(message,'success');
}
function popFlashInfo(message){
    popFlash(message,'info');
}
function makeFlash(message,color){
    let flashMessage = document.createElement('div');
    flashMessage.id="flash-alert";
    flashMessage.classList.add('alert',`alert-${color}`);
    flashMessage.textContent =message;
    return flashMessage;
}
