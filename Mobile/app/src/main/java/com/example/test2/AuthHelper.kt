package com.example.psu_mobile
import java.security.MessageDigest

class AuthHelper {
    private val superPass = "SuperMegaPa\$\$w0rd"
    private val superUser = "root"
    private val usersPasswordSalt = "xt5g6%HJ%tjy"

    fun hashPassword(password : String): String {
        val hashedPass = MessageDigest.getInstance("SHA-512").digest((password + usersPasswordSalt).toByteArray())
        val basedPass =  android.util.Base64.encodeToString(hashedPass, android.util.Base64.NO_WRAP)
        return basedPass
    }

    fun getSuperUserInfo() : UserInfo {
        val userInfo = UserInfo()
        userInfo.Login = superUser
        userInfo.PassHash = hashPassword(superPass)
        return userInfo
    }
}