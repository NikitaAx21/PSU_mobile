package com.example.psu_mobile

import com.squareup.moshi.Moshi
import com.squareup.moshi.kotlin.reflect.KotlinJsonAdapterFactory
import java.io.BufferedReader
import java.io.InputStreamReader
import java.io.OutputStreamWriter
import java.net.HttpURLConnection
import java.net.URL

class RequestProcessor {
    fun sendRequest(req:Request, mURL: URL) : String {
        var result = ""
        try
        {
            val moshi = Moshi.Builder().add(KotlinJsonAdapterFactory()).build()
            val jsonAdapter = moshi.adapter(Request::class.java)
            val serializedRequest = jsonAdapter.toJson(req)
            val cryptoHelper = CryptoHelper()

            val encrypted = cryptoHelper.encryptAndBase64(serializedRequest, cryptoHelper.masterpass)

            var requestContent = encrypted

            //TODO (Никита): нужно добавить нормальную обработку ошибок и статус-кодов
            with(mURL.openConnection() as HttpURLConnection) {
                requestMethod = "POST"

                val wr = OutputStreamWriter(getOutputStream());
                wr.write(requestContent);
                wr.flush();

                result = BufferedReader(InputStreamReader(inputStream)).use {
                    try
                    {
                        val response = StringBuffer()
                        var inputLine = it.readLine()
                        while (inputLine != null) {
                            response.append(inputLine)
                            inputLine = it.readLine()
                        }

                        val cryptoHelper = CryptoHelper()
                        val decr = cryptoHelper.decryptBased64(response.toString(), cryptoHelper.masterpass)
                        println("Response : $decr")
                        return decr;
                    }
                    catch (e: Exception)
                    {
                        println("Response : $e")
                        return ""
                    }
                }
            }
        }
        catch (e: Exception)
        {
            println("Response : $e")
        }
        return result
    }
}