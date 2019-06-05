<?php
/**
 * Created by PhpStorm.
 * User: Medhi
 * Date: 28/02/2019
 * Time: 17:59
 */

session_start();
require_once "../../utils/header.php"
?>

<div class="container">
    <h3>Création de compte :</h3>
    <form method="post" action="../../services/account/insert.php">
        <div class="form-group">
            <label for="accountName">Nom de compte :</label>
            <input type="text" class="form-control" id="accountName" placeholder="Votre nom de compte">
        </div>
        <div class="form-group">
            <label for="accountMail">Email :</label>
            <input type="email" class="form-control" id="accountMail" placeholder="Votre email">
        </div>
        <div class="form-group">
            <label for="accountPassword">Mot de passe :</label>
            <input type="password" class="form-control" id="accountPassword" placeholder="Votre mot de passe">
        </div>
        <div class="form-group">
            <label for="accountPassword">Confirmation :</label>
            <input type="password" class="form-control" id="accountPasswordConfirmation" placeholder="Votre mot de passe">
        </div>
        <button id="submit-button" type="submit" class="btn btn-primary">Créer mon compte</button>
    </form>
</div>

<?php
require_once "../../utils/footer.php"
?>
