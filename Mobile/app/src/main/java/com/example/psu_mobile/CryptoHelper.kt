package com.example.psu_mobile

import java.io.*
import java.security.MessageDigest
import java.security.SecureRandom
import javax.crypto.Cipher
import javax.crypto.CipherOutputStream
import javax.crypto.spec.IvParameterSpec
import javax.crypto.spec.SecretKeySpec

class CryptoHelper {
    val masterpass = "0H;#4c,lt`F! ?: 46ZI* J1ZfGXi:d38"

    fun encrypt(strToEncrypt: String, password : String) : ByteArray
    {
        try
        {
            val cipher = Cipher.getInstance("AES/CBC/PKCS7Padding")

            val randomSecureRandom = SecureRandom.getInstance("SHA1PRNG")
            val iv = ByteArray(cipher.blockSize)
            randomSecureRandom.nextBytes(iv)
            val ivParameterSpec = IvParameterSpec(iv)

            val hashedPass = MessageDigest.getInstance("SHA-256").digest(password.toByteArray())
            val secretKey =  SecretKeySpec(hashedPass, "AES")

            cipher.init(Cipher.ENCRYPT_MODE, secretKey, ivParameterSpec)
            var encrypted = cipher.doFinal(strToEncrypt.toByteArray(Charsets.UTF_8))
            val resultString = iv + encrypted
            return resultString
        }
        catch (e: Exception)
        {
            println("Error while encrypting: $e")
        }
        return "".toByteArray()
    }

    fun encrypt(strToEncrypt: InputStream, password : String, outputStream: OutputStream)
    {
        try
        {
            val cipher = Cipher.getInstance("AES/CBC/PKCS7Padding")

            val randomSecureRandom = SecureRandom.getInstance("SHA1PRNG")
            val iv = ByteArray(cipher.blockSize)
            randomSecureRandom.nextBytes(iv)
            val ivParameterSpec = IvParameterSpec(iv)

            val hashedPass = MessageDigest.getInstance("SHA-256").digest(password.toByteArray())
            val secretKey =  SecretKeySpec(hashedPass, "AES")

            cipher.init(Cipher.ENCRYPT_MODE, secretKey, ivParameterSpec)

            outputStream.write(iv)

            var cipherOutputStream = CipherOutputStream(outputStream, cipher)
            var b = strToEncrypt.read()
            while (b > 0)
            {
                cipherOutputStream.write(b)
                b = strToEncrypt.read()
            }
            cipherOutputStream.close()
        }
        catch (e: Exception)
        {
            println("Error while encrypting: $e")
        }
    }

    fun encryptAndBase64(strToEncrypt: String, password : String) :  String
    {
        val encrypted = encrypt(strToEncrypt, password)
        return android.util.Base64.encodeToString(encrypted, android.util.Base64.NO_WRAP)
    }

    fun decrypt(source: ByteArray, password : String) : String {
        try
        {
            val cipher = Cipher.getInstance("AES/CBC/PKCS7Padding")

            val iv = source.copyOfRange(0, cipher.blockSize)
            val encrypted = source.copyOfRange(cipher.blockSize, source.count())

            val ivParameterSpec = IvParameterSpec(iv)

            val sha256Maker = MessageDigest.getInstance("SHA-256")
            val hashedPass = sha256Maker.digest(password.toByteArray())
            val secretKey =  SecretKeySpec(hashedPass, "AES")

            cipher.init(Cipher.DECRYPT_MODE, secretKey, ivParameterSpec);

            return String(cipher.doFinal(encrypted))
        }
        catch (e : Exception)
        {
            println("Error while decrypting: $e");
        }
        return ""
    }

    fun decrypt(source: InputStream, password : String, outputStream: OutputStream) {
        try
        {
            val cipher = Cipher.getInstance("AES/CBC/PKCS7Padding")

            var iv = ByteArray(cipher.blockSize)
            source.read(iv, 0, cipher.blockSize)

            val ivParameterSpec = IvParameterSpec(iv)

            val sha256Maker = MessageDigest.getInstance("SHA-256")
            val hashedPass = sha256Maker.digest(password.toByteArray())
            val secretKey =  SecretKeySpec(hashedPass, "AES")

            cipher.init(Cipher.DECRYPT_MODE, secretKey, ivParameterSpec)

            var cipherOutputStream = CipherOutputStream(outputStream, cipher)
            var b = source.read()
            while (b > 0)
            {
                cipherOutputStream.write(b)
                b = source.read()
            }
            cipherOutputStream.close()
        }
        catch (e : Exception)
        {
            println("Error while decrypting: $e");
        }
    }

    fun decryptBased64(strToDecrypt : String, password : String) : String
    {
        val encrypted = android.util.Base64.decode(strToDecrypt, android.util.Base64.NO_WRAP)
        return decrypt(encrypted, password)
    }
}