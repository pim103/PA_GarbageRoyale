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

<div class="container" xmlns="http://www.w3.org/1999/html">

    <h3>Me connecter</h3>
    <form method="post" action="">
        <div class="form-group">
            <label for="accountName">Nom de compte :</label>
            <input type="text" class="form-control" id="accountName" placeholder="Votre nom de compte">
        </div>
        <div class="form-group">
            <label for="accountPassword">Mot de passe :</label>
            <input type="password" class="form-control" id="accountPassword" placeholder="Votre mot de passe">
        </div>
        <button id="submit-button" type="submit" class="btn btn-primary">Se connecter</button>
    </form>
    <div class="create-account-div">
        <p style="float: left;">Pas encore de compte ? </p>
        <a id="create-account" href="pages/account/register.php">Créer son compte</a>
    </div>
</div>

<?php
require_once "./utils/footer.php"
?>
