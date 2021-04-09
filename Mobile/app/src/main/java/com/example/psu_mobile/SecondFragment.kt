package com.example.psu_mobile

import android.os.Bundle
import android.os.StrictMode
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import androidx.fragment.app.Fragment
import androidx.navigation.fragment.findNavController
import com.squareup.moshi.Moshi
import com.squareup.moshi.kotlin.reflect.KotlinJsonAdapterFactory
import java.io.ByteArrayInputStream
import java.net.URL
import kotlin.random.Random

/**
 * A simple [Fragment] subclass as the second destination in the navigation.
 */
class SecondFragment : Fragment() {

    override fun onCreateView(
            inflater: LayoutInflater, container: ViewGroup?,
            savedInstanceState: Bundle?
    ): View? {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_second, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        view.findViewById<Button>(R.id.button_second).setOnClickListener {
            findNavController().navigate(R.id.action_SecondFragment_to_FirstFragment)

            //TODO (Никита): Лень было думать, откуда вызывать. Тупоприципился к кнопке "предыдущая активность"
            sendPostRequestAsync()
        }
    }

    fun sendPostRequestAsync() {
//TODO (Никита): Супер костыль - запуск в основном потоке. Нужно сделать асинхронным
        val policy = StrictMode.ThreadPolicy.Builder().permitAll().build()
        StrictMode.setThreadPolicy(policy)

//TODO (Никита): Ниже просто пример того, как должны будут устроены запросы. Пока не трогал их. Нужно вынести
        val authHelper = AuthHelper()
        val pass = "SomePass"
        val newUserInfo = UserInfo()
        newUserInfo.Login = "User_${Random.nextInt()}"
        newUserInfo.PassHash = authHelper.hashPassword(pass)

        val jsonAdapter =  Moshi.Builder().add(KotlinJsonAdapterFactory()).build().adapter(UserInfo::class.java)
        val reqContent = jsonAdapter.toJson(newUserInfo)
        val reqMethod = "CreateUser"

        val req = Request()
        req.User = authHelper.getSuperUserInfo()
        req.Method = reqMethod
        var seq = ByteArrayInputStream(reqContent.toByteArray())

//TODO (Никита): костыль тупо хардкод локального адреса моего (Никита) компа. Нужно будет продумать этот момент
        val mURL = URL("http://192.168.50.224:8888")
        val response = RequestProcessor().sendRequest(req, seq, mURL)
//TODO (Никита): пример получения текстового ответа. Файлы, конечно, в массивы байт перегонять не стоит
        val r = String(response.toByteArray())
        println("Response : $r")
    }
}