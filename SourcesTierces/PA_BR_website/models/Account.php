<?php
/**
 * Created by PhpStorm.
 * User: Medhi
 * Date: 28/02/2019
 * Time: 19:18
 */

class Account implements JsonSerializable
{
    private $id;
    private $account_name;
    private $account_email;
    private $account_password;
    private $account_token;
    private $account_userid;
    private $account_role;
    private $is_active;
    private $score;

    /**
     * Account constructor.
     * @param $id
     * @param $account_name
     * @param $account_email
     * @param $account_password
     * @param $account_token
     * @param $account_userid
     * @param $account_role
     * @param $is_active
     * @param $score
     */
    public function __construct($id, $account_name, $account_email, $account_password, $account_token, $account_userid, $account_role, $is_active, $score)
    {
        $this->id = $id;
        $this->account_name = $account_name;
        $this->account_email = $account_email;
        $this->account_password = $account_password;
        $this->account_token = $account_token;
        $this->account_userid = $account_userid;
        $this->account_role = $account_role;
        $this->is_active = $is_active;
        $this->score = $score;
    }


    public function jsonSerialize()
    {
        return get_object_vars($this);
    }

    /**
     * @return mixed
     */
    public function getId()
    {
        return $this->id;
    }

    /**
     * @param mixed $id
     */
    public function setId($id)
    {
        $this->id = $id;
    }

    /**
     * @return mixed
     */
    public function getAccountName()
    {
        return $this->account_name;
    }

    /**
     * @param mixed $account_name
     */
    public function setAccountName($account_name)
    {
        $this->account_name = $account_name;
    }

    /**
     * @return mixed
     */
    public function getAccountEmail()
    {
        return $this->account_email;
    }

    /**
     * @param mixed $account_email
     */
    public function setAccountEmail($account_email)
    {
        $this->account_email = $account_email;
    }

    /**
     * @return mixed
     */
    public function getAccountPassword()
    {
        return $this->account_password;
    }

    /**
     * @param mixed $account_password
     */
    public function setAccountPassword($account_password)
    {
        $this->account_password = $account_password;
    }

    /**
     * @return mixed
     */
    public function getAccountToken()
    {
        return $this->account_token;
    }

    /**
     * @param mixed $account_token
     */
    public function setAccountToken($account_token)
    {
        $this->account_token = $account_token;
    }

    /**
     * @return mixed
     */
    public function getAccountUserid()
    {
        return $this->account_userid;
    }

    /**
     * @param mixed $account_userid
     */
    public function setAccountUserid($account_userid)
    {
        $this->account_userid = $account_userid;
    }

    /**
     * @return mixed
     */
    public function getAccountRole()
    {
        return $this->account_role;
    }

    /**
     * @param mixed $account_role
     */
    public function setAccountRole($account_role)
    {
        $this->account_role = $account_role;
    }

    /**
     * @return mixed
     */
    public function getIsActive()
    {
        return $this->is_active;
    }

    /**
     * @param mixed $is_active
     */
    public function setIsActive($is_active)
    {
        $this->is_active = $is_active;
    }

    /**
     * @return mixed
     */
    public function getScore()
    {
        return $this->score;
    }

    /**
     * @param mixed $score
     */
    public function setScore($score)
    {
        $this->score = $score;
    }
}
