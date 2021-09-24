const labels = document.querySelectorAll("label");
const form = document.querySelector("form")
const inputText = document.getElementsByClassName("container-input")
console.log(form);
console.log(inputText);


async function start(){
    const API_KEY = "?key=23497905-ebfa1b80f325f860fce195702";
    let url = "https://pixabay.com/api/" + API_KEY + "&q";
    let respons = await fetch(url);
    let json = await respons.json();

    console.log(json)
}

form.onsubmit = event => {
    event.preventDefault();

    const params = form.elements.text.value;
    const colorCheckBoxes = Array.from(document.querySelectorAll(".container-color-labels input[type=checkbox]"));
    const activColorCheckboxes = colorCheckBoxes.filter(c => c.checked);
    const activColorValue = activColorCheckboxes.map(c => c.value);
    console.log(params);
    console.log(colorCheckBoxes);
    console.log(activColorValue);
}

start()

function checkboxCSSChange(color){
    console.log(color.value)
    let labelcolor = document.getElementById("label-" + color.value);
    let inputcolor = document.getElementById("input-color-" + color.value);

    let selectedColor = color.value;
    if(inputcolor.checked){
        labelcolor.style.backgroundColor=selectedColor
        labelcolor.style.opacity = 1;
    }
    if(inputcolor.checked != true){
        labelcolor.style.opacity = 0.5;
    }
}

function checkboxMousEnterEvent(label){
    let labelcolor = document.getElementById(label.id);
    let inputcolor = document.querySelector("#"+label.id + " input");
    if(inputcolor.checked != true){
        labelcolor.style.opacity = 1;
    }
        
}

function checkboxMousLeaveEvent(label){
    let labelcolor = document.getElementById(label.id);
    let inputcolor = document.querySelector("#"+label.id + " input");
    if(inputcolor.checked != true){
        labelcolor.style.opacity = 0.5;
    }
}
