// function myJsFunction() {
//   Html5Qrcode.getCameras().then(devices => {
//     /**
//      * devices would be an array of objects of type:
//      * { id: "id", label: "label" }
//      */
//     if (devices && devices.length) {
//       var cameraId = devices[0].id;
//       const html5QrCode = new Html5Qrcode(/* element id */ "reader");
//       html5QrCode.start(
//         cameraId, 
//         {
//           fps: 10,    // Optional, frame per seconds for qr code scanning
//           qrbox: { width: 250, height: 250 }  // Optional, if you want bounded box UI
//         },
//         (decodedText, decodedResult) => {
//           console.log("decodedText", decodedText);
//           // do something when code is read
//         },
//         (errorMessage) => {
//           // parse error, ignore it.
//         })
//       .catch((err) => {
//         // Start failed, handle it.
//       });

//       // .. use this to start scanning.
//     }
//   }).catch(err => {
//     // handle err
//   });
// }
let html5QrCode
let cameraId
let configUI

function setupQrCodeScanner() {
  Html5Qrcode.getCameras().then(devices => {
    /**
     * devices would be an array of objects of type:
     * { id: "id", label: "label" }
     */
    configUI = {
      fps: 10,
      videoConstraints: {
      },
      qrbox: (w, h) => {
          return {
              width: 250,
              height: 250,
          }
      },
   }
    if (devices && devices.length) {
      cameraId = devices[0].id;
      html5QrCode = new Html5Qrcode(/* element id */ "reader");
      html5QrCode.start(
        cameraId, 
        configUI,
        (decodedText, decodedResult) => {
          // console.log("decodedText", decodedText);
          DotNet.invokeMethodAsync('BlazorIntAuto.Client', 'ReceiveDecodedText', decodedText);
          console.log(decodedResult);
          html5QrCode.stop().then(ignore => {
            // QR Code scanning is stopped.
          }).catch(err => {
            // Stop failed, handle it.
          });
          // do something when code is read
        },
        (errorMessage) => {
          // parse error, ignore it.
        })
      .catch((err) => {
        // Start failed, handle it.
      });

      // .. use this to start scanning.
    }
  }).catch(err => {
    // handle err
  });
};

function startQrCodeScanner() {
  if(!html5QrCode) {
    return
  }
  html5QrCode.start(
    cameraId, 
    configUI,
    (decodedText, decodedResult) => {
      console.log("decodedText", decodedText);
      // do something when code is read
    },
    (errorMessage) => {
      // parse error, ignore it.
    })
  .catch((err) => {
    // Start failed, handle it.
  });
}

function stopQrCodeScanner() {
  if(!html5QrCode) {
    return
  }
  html5QrCode.stop().then(ignore => {
    // QR Code scanning is stopped.
  }).catch(err => {
    // Stop failed, handle it.
  });
}


window.myJsFunction = () => {
  alert(cameraId);
}