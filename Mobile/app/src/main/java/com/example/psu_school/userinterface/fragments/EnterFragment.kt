package com.example.psu_school.userinterface.fragments

import android.os.StrictMode
import androidx.fragment.app.Fragment
import com.example.psu_mobile.AuthHelper
import com.example.psu_mobile.Request
import com.example.psu_mobile.RequestProcessor
import com.example.psu_mobile.UserInfo
import com.example.psu_school.R
import com.squareup.moshi.Moshi
import com.squareup.moshi.kotlin.reflect.KotlinJsonAdapterFactory
import kotlinx.android.synthetic.main.fragment_enter.*
import utilits.replace_fragment
import utilits.show_toast
import java.io.ByteArrayInputStream
import java.net.URL
import kotlin.random.Random

@Suppress("DEPRECATION")
class EnterFragment : Fragment(R.layout.fragment_enter) {
    override fun onStart() {
        super.onStart()
        register_input_button.setOnClickListener {send_auth_params()}
    }

    private fun send_auth_params() {
        if(register_input_login.text.toString().isEmpty()){
            show_toast(getString(R.string.register_toast_login))
        }
        else if(register_input_pass.text.toString().isEmpty()){
            show_toast(getString(R.string.register_toast_pass))
        }
        else{
            //здесь надо проводить авторизацию со всеми вытекающими
              //дальше этого места приложение не пойдет
            //sendPostRequestAsync()
            replace_fragment(MailFragment())
        }
    }

    fun sendPostRequestAsync() {
        try {
            //TODO (Никита): Супер костыль - запуск в основном потоке. Нужно сделать асинхронным
            val policy = StrictMode.ThreadPolicy.Builder().permitAll().build()
            StrictMode.setThreadPolicy(policy)

//TODO (Никита): Ниже просто пример того, как должны будут устроены запросы. Пока не трогал их. Нужно вынести
            val authHelper = AuthHelper()
            val pass = "SomePass"
            val newUserInfo = UserInfo()
            newUserInfo.Login = "User_${Random.nextInt()}"
            newUserInfo.PassHash = authHelper.hashPassword(pass)

            val jsonAdapter =  Moshi.Builder().add(KotlinJsonAdapterFactory()).build().adapter(
                UserInfo::class.java
            )
            val reqContent = jsonAdapter.toJson(newUserInfo)
            val reqMethod = "CreateUser"

            val req = Request()
            req.User = authHelper.getSuperUserInfo()
            req.Method = reqMethod
            var seq = ByteArrayInputStream(reqContent.toByteArray())

//TODO (Никита): костыль тупо хардкод локального адреса моего (Никита) компа. Нужно будет продумать этот момент
            val mURL = URL("http://192.168.0.3:8888/")
            val response = RequestProcessor().sendRequest(req, seq, mURL)
//TODO (Никита): пример получения текстового ответа. Файлы, конечно, в массивы байт перегонять не стоит
            val r = String(response.toByteArray())
            println("Response : $r")
        }
        catch (e: NumberFormatException){
            throw RuntimeException("fuck your: ", e)
        }
    }
}