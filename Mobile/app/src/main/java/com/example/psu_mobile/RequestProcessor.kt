package com.example.psu_mobile

import com.squareup.moshi.Moshi
import com.squareup.moshi.kotlin.reflect.KotlinJsonAdapterFactory
import java.io.*
import java.net.HttpURLConnection
import java.net.URL

class RequestProcessor {
    fun sendRequest(req:Request, content: InputStream, mURL: URL) : ByteArrayOutputStream {
        try
        {
            val jsonAdapter = Moshi.Builder()
                    .add(KotlinJsonAdapterFactory())
                    .build()
                    .adapter(Request::class.java)
            val serializedRequest = jsonAdapter.toJson(req)
            val cryptoHelper = CryptoHelper()

            val encrypted = cryptoHelper.encryptAndBase64(serializedRequest, cryptoHelper.masterpass)
            //TODO (Никита): нужно добавить нормальную обработку ошибок и статус-кодов
            with(mURL.openConnection() as HttpURLConnection) {
                return try {
                    requestMethod = "POST"
                    setRequestProperty("X-Request", encrypted)
                    cryptoHelper.encrypt(content, cryptoHelper.masterpass, outputStream)
                    outputStream.flush()

                    var response = ByteArrayOutputStream()
                    cryptoHelper.decrypt(inputStream, cryptoHelper.masterpass, response)
                    response
                } catch (e: Exception) {
                    println("Response : $e")
                    null!!
                }
            }
        }
        catch (e: Exception)
        {
            println("Response : $e")
        }
        return null!!
    }
}