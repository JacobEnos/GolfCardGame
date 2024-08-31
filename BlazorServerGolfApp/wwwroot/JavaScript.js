function bounceIcon(elementId) {
    const element = document.getElementById(elementId);
    const initialPosition = parseInt(getComputedStyle(element).bottom);
    const bounceHeight = 20;
    const bounceDuration = 500;
    const gravity = 9.8;

    let startTime;
    let currentPosition = initialPosition;

    function animate(timestamp) {
        if (!startTime) {
            startTime = timestamp;
        }

        const elapsed = timestamp - startTime;
        const bounceProgress = (elapsed % bounceDuration) / bounceDuration;
        const bounceDistance = Math.sin(bounceProgress * Math.PI) * bounceHeight;
        currentPosition = initialPosition - bounceDistance + (gravity * elapsed * elapsed) / 2000;

        element.style.bottom = currentPosition + "px";

        if (currentPosition > initialPosition) {
            requestAnimationFrame(animate);
        } else {
            element.style.bottom = initialPosition + "px";
        }
    }

    requestAnimationFrame(animate);
}



function loseLife(losersString) {

    var lives = document.getElementById("LifeContainer");
    return bounceIcon(lives.firstChild.elementId);
}

function showDiag(){
    var d = document.getElementById('popup');
    d.close();
    d.showModal();
}

function closeDiag() {
    var d = document.getElementById('popup');
    d.close();
}


function showScoreboard(scoreboardHTML) {
    if (scoreboardHTML == null || scoreboardHTML.length < 1) {
        scoreboardHTML = "No scores yet"
    }

    var d = document.getElementById('scoreboardWrapper');
    d.innerHTML = scoreboardHTML;

    var popup = document.getElementById('scoreboardWindow');
    popup.showModal();
}

function closeScoreboard() {
    var popup = document.getElementById('scoreboardWindow');
    popup.close();
}

function chooseAvatar() {

    var popup = document.getElementById('avatar-popup');
    popup.close();
    popup.showModal();
    return;// bounceIcon();
}


function closeAvatarDiag() {
    var d = document.getElementById('avatar-popup');
    d.close();
}


function alert(message) {
    window.alert(message);
}

function toggleChat() {
    var chatWindow = document.getElementById("chatWindow");
    chatWindow.style.display = chatWindow.style.display == "block" ? "none" : "block"
}

function addChat(message){
    var chatWindow = document.getElementById("chatWindow");
    chatWindow.style.display = "block";
    if (1 < chatWindow.innerHTML.length) {
        chatWindow.innerHTML = "<br>" + chatWindow.innerHTML
    }
    chatWindow.innerHTML = message + chatWindow.innerHTML;
}

function toggleHandPreview(divID) {
    var div = document.getElementById(divID);
    if (!div) {
        return;
    }
    div.style.display = div.style.display == "none" ? "block" : "none";
}
