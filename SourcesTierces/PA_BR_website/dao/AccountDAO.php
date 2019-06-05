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
        $characters = '0123456789abcdefghijklmnopqrstuvwxyz';
        $charactersLength = strlen($characters);
        $userid = '';
        for ($i = 0; $i < 8; $i++) {
            $userid .= $characters[rand(0, $charactersLength - 1)];
        }
        $userid .= "-";
        for ($j = 0; $j < 4; $j++) {
            $userid .= $characters[rand(0, $charactersLength - 1)];
        }
        $userid .= "-";
        for ($j = 0; $j < 4; $j++) {
            $userid .= $characters[rand(0, $charactersLength - 1)];
        }
        $userid .= "-";
        for ($j = 0; $j < 4; $j++) {
            $userid .= $characters[rand(0, $charactersLength - 1)];
        }
        $userid .= "-";
        for ($i = 0; $i < 12; $i++) {
            $userid .= $characters[rand(0, $charactersLength - 1)];
        }

        return $userid;
    }

    public static function createAccount($name, $email, $password){
        $db = DatabaseManager::getSharedInstance();
        $userid = AccountDAO::createUserId();
        $query = $db->exec('INSERT INTO account (email, name, password, userid) VALUES (?, ?, ?, ?)', [$email, $name, $password, $userid]);
        return $query != 0;
    }

    public static function updatePassword($id, $password){
        $db = DatabaseManager::getSharedInstance();
        $sql = 'UPDATE account SET password=? WHERE id=?';
        $query = $db->getPDO()->prepare($sql);
        $updated = $query->execute([$password, $id]);

        return $updated != 0;
    }

    public static function disableAccount($id){
        $db = DatabaseManager::getSharedInstance();
        $sql = 'UPDATE account SET isActive=0 WHERE id=?';
        $query = $db->getPDO()->prepare($sql);
        $disabled = $query->execute([$id]);

        return $disabled != 0;
    }

    public static function listPlayerScores(){
        $db = DatabaseManager::getSharedInstance();
        $result = $db->getAll("SELECT name, score FROM account ORDER BY score DESC");

        return $result;
    }
}
