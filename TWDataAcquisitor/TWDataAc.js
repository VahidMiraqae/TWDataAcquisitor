
var theTimeout = null;
var samplingCount = 0;
var sessionText = "";

var dateTimeEl = document.querySelectorAll("td.time-axis>div>canvas")[1];

function download(filename, text) {
      var element = document.createElement('a');
      element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
      element.setAttribute('download', filename);
    
      element.style.display = 'none';
      document.body.appendChild(element);
    
      element.click();
    
      document.body.removeChild(element);
}

function keyDownFunction(e) {

    if(e.key != "ArrowLeft")
        return;

    clearTimeout(theTimeout);
    
    if (samplingCount == 0) {
        sessionText = document.querySelector("div[class^='titleWrapper']").innerText + '\n';
    } 

    sessionText += document.querySelector("div[class^='valuesAdditionalWrapper']").innerText;
    sessionText = sessionText + "|" + dateTimeEl.toDataURL().substring(22) + '\n';

    samplingCount++;

    if (samplingCount == 30){
        download("chart_data.txt", sessionText);
        samplingCount = 0;
    }

    theTimeout = setTimeout(function() { samplingCount = 0; console.log("timeout cleared!"); }, 5000);
}

document.addEventListener("keydown",  keyDownFunction); 