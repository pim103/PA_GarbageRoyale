<?php
/**
 * Created by PhpStorm.
 * User: Medhi
 * Date: 28/02/2019
 * Time: 17:59
 */

session_start();
require_once "./utils/header.php"
?>
<div id="download-div">
    <a class="name-div" style="text-decoration: none; color: #ffdd0b;" href="http://garbage-royale.heolia.eu/files/games/GarbageRoyale.zip">
        <p class="name-div" style="text-decoration: none; color: #ffdd0b;">Télécharger le jeu </p>
        <i class="fas fa-download">
        </i>
    </a>
</div>



<div id="main-div">
    <div id="titles-div">
        <h1>Garbage Royale</h1>
        <h2>Scoreboard</h2>
    </div>
    <div id="scoreboard-div">

    </div>
</div>

<script src="view/models/Score.js"></script>
<script src="view/services/ScoreService.js"></script>
<script src="view/fetchScore.js"></script>
<style>
    @font-face {font-family:"Orbitron Black";src:url("files/fonts/orbitron-black.eot?") format("eot"),url("files/fonts/orbitron-black.woff") format("woff"),url("files/fonts/orbitron-black.ttf") format("truetype"),url("files/fonts/orbitron-black.svg#Orbitron-Black") format("svg");font-weight:normal;font-style:normal;}

    body{
        background: #041057;
    }
    #main-div{
        background: black;
        margin: auto;
        border-radius: 25px;
        width: 80%;
    }
    #download-div{
        color: #FFFFFF;
        width: 100%;
        margin: auto;
        text-align: center;
        font-family: Orbitron Black;
        padding-bottom: 10px;
        display: inline-flex;
    }
    #titles-div{
        margin-bottom: 40px;
    }
    #score-div{
        color: #FFFFFF;
        margin: auto;
        text-align: center;
    }
    .name-div{
        color: #FFFFFF;
        margin: auto;
        text-align: center;
        font-family: Orbitron Black;
        padding-bottom: 10px;
        display: inline-flex;
    }
    h1{
        color: #FFFFFF;
        margin: auto;
        text-align: center;
        font-family: Orbitron Black;
    }
    h2{
        color: #FFFFFF;
        margin: auto;
        text-align: center;
        font-family: Orbitron Black;
    }
</style>

<?php
require_once "./utils/footer.php"
?>
