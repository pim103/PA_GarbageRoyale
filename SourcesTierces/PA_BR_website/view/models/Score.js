let Score = function(name, score) {
    this.name = name;
    this.score = score;
};

Score.fromJSON = function(obj) {
    return new Score(
        obj.name,
        obj.score
    );
};

Score.prototype.toHTML = function(){
    let parent = document.createElement("div");
    parent.className = "individual-score-div";
    let nameDiv = document.createElement("div");
    nameDiv.innerText = "" + this.name +  " - Score : " + this.score;;
    nameDiv.className = "name-div";

    parent.appendChild(nameDiv);

    return parent;
};