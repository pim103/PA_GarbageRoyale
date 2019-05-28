<?php
/**
 * Created by PhpStorm.
 * User: Medhi
 * Date: 28/02/2019
 * Time: 20:28
 */

require_once __DIR__ ."/../models/Account.php";
require_once __DIR__ ."/../utils/functions.php";
require_once __DIR__ ."/../utils/DatabaseManager.php";

ini_set('display_errors', 1);


class AccountDAO
{
    private static function accountFromRows(&$rows)
    {
        $arr = [];
        foreach ($rows as &$row) {
            $arr[] = AccountDAO::accountFromRow($row);
        }
        return $arr;
    }

    private static function accountFromRow(&$row)
    {
        $account = new Account(
            $row['id'],
            $row['name'],
            $row['email'],
            $row['password'],
            $row['token'],
            $row['userid'],
            $row['role'],
            $row['isActive'],
            $row['score']
        );

        return $account;
    }

    private static function createUserId(){
        $characters = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
        $charactersLength = strlen($characters);
        $userid = '';
        for ($i = 0; $i < 8; $i++) {
            $userid .= $characters[rand(0, $charactersLength - 1)];
        }
        $userid .= "-";
        for ($i = 0; $i < 3; $i++) {
            for ($i = 0; $i < 4; $i++) {
                $userid .= $characters[rand(0, $charactersLength - 1)];
            }
            $userid .= "-";
        }
        for ($i = 0; $i < 12; $i++) {
            $userid .= $characters[rand(0, $charactersLength - 1)];
        }

        return $userid;
    }

    public static function createAccount($name, $email, $password){
        $db = DatabaseManager::getSharedInstance();
        $userid = AccountDAO::createUserId();
        $query = $db->exec('INSERT INTO account (account_email, account_nickname, account_password, account_token, account_userid) VALUES (?, ?, ?, ?, ?)', [$email, $name, $password, null, $userid]);

        return $query != 0;
    }
}
