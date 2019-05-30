<?php

ini_set('display_errors', 1);

require_once "../../utils/DatabaseManager.php";

if (count( $_POST ) == 2 && ! empty( $_POST["accountEmail"] ) && ! empty( $_POST["accountPassword"] ) ) {
    $db = DatabaseManager::getSharedInstance();
    $error = false;
    $listOfErrors = [];
    $query = $db->getPDO()->prepare('SELECT password FROM account WHERE email="' . $_POST["accountEmail"].'"');
    $query->execute();
    $result = $query->fetch();
    //print_r($result);

    if (password_verify($_POST["accountPassword"], $result[0])) {
        $queryUserId =$db->getPDO()->prepare('SELECT userid FROM account WHERE email="' . $_POST["accountEmail"].'"');
        $queryUserId->execute();
        $userId = $queryUserId->fetch();
        echo $userId[0];
        http_response_code(202);
    } else {
        http_response_code(500);
    }
} else {
    http_response_code(500);
}
