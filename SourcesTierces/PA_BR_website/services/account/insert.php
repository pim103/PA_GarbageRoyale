<?php
/**
 * Created by PhpStorm.
 * User: Medhi
 * Date: 01/03/2019
 * Time: 23:50
 */

ini_set('display_errors', 1);

require_once __DIR__ ."/../../dao/AccountDAO.php";


if( count($_POST) >= 4 &&
    !empty($_POST["accountName"]) &&
    !empty($_POST["accountMail"]) &&
    !empty($_POST["accountPassword"]) &&
    !empty($_POST["accountPasswordConfirmation"])){

    $error = false;
    $listOfErrors = [];

    if(strlen($_POST["accountName"]) < 3  || strlen($_POST["accountName"]) > 14 ){
        $error = true;
        $listOfErrors[] = 1;
    }

    if(!filter_var($_POST["accountMail"], FILTER_VALIDATE_EMAIL )){
        $error = true;
        $listOfErrors[] = 2;
    }

    if(strlen($_POST["accountPassword"]) < 8  || strlen($_POST["accountPassword"]) > 24 ){
        $error = true;
        $listOfErrors[] = 3;
    }

    if($_POST["accountPassword"] != $_POST["accountPasswordConfirmation"] ){
        $error = true;
        $listOfErrors[] = 4;
    }

    if($error) {
        $success = null;
        //print_r($listOfErrors);
    }
    else {
        //print_r($listOfErrors);
        $passHash = password_hash($_POST["accountPassword"], PASSWORD_DEFAULT);
        $success = AccountDAO::createAccount($_POST['accountName'], $_POST["accountMail"], $passHash);
    }

    if($success){
        http_response_code(201);
        //echo "0";
    }
    else{
        //echo "1 - " . $_POST["accountName"];
        http_response_code(500);
    }


}
