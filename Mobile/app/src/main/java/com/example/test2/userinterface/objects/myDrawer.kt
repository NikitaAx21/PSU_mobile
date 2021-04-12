package com.example.test2.userinterface.objects

import android.app.Activity
import android.content.Intent
import android.os.Bundle
import android.os.StrictMode
import android.view.View
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import androidx.core.content.ContextCompat.startActivity
import com.example.psu_mobile.AuthHelper
import com.example.psu_mobile.Request
import com.example.psu_mobile.RequestProcessor
import com.example.psu_mobile.UserInfo
import com.example.test2.MapsActivity
import com.example.test2.R
import com.example.test2.userinterface.fragments.BaseFragment
import com.google.zxing.integration.android.IntentIntegrator
import com.mikepenz.materialdrawer.AccountHeader
import com.mikepenz.materialdrawer.AccountHeaderBuilder
import com.mikepenz.materialdrawer.Drawer
import com.mikepenz.materialdrawer.DrawerBuilder
import com.mikepenz.materialdrawer.model.PrimaryDrawerItem
import com.mikepenz.materialdrawer.model.ProfileDrawerItem
import com.mikepenz.materialdrawer.model.interfaces.IDrawerItem
import com.squareup.moshi.Moshi
import com.squareup.moshi.kotlin.reflect.KotlinJsonAdapterFactory
import java.io.ByteArrayInputStream
import java.net.URL
import kotlin.random.Random

class myDrawer(val mainActivity: AppCompatActivity, val toolbar: Toolbar){
    private lateinit var mDrawer: Drawer
    private lateinit var mHeader: AccountHeader

    public fun createDrawer(){
        createHeader()
        createSubDrawer()
    }

    private fun createSubDrawer() {
        mDrawer = DrawerBuilder()
            .withActivity(mainActivity)
            .withToolbar(toolbar)
            .withActionBarDrawerToggle(true)
            .withSelectedItem(-1)
            .withAccountHeader(mHeader)
            .addDrawerItems(
                PrimaryDrawerItem().withIdentifier(100)
                    .withIconTintingEnabled(true)
                    .withName("Новости")
                    .withSelectable(false)
                    .withIcon(R.drawable.ico_news),
                PrimaryDrawerItem().withIdentifier(101)
                    .withIconTintingEnabled(true)
                    .withName("Расписание")
                    .withSelectable(false)
                    .withIcon(R.drawable.ico_timetable),
                PrimaryDrawerItem().withIdentifier(102)
                    .withIconTintingEnabled(true)
                    .withName("Домашнее задание")
                    .withSelectable(false)
                    .withIcon(R.drawable.ico_homework),
                PrimaryDrawerItem().withIdentifier(103)
                    .withIconTintingEnabled(true)
                    .withName("Почта")
                    .withSelectable(false)
                    .withIcon(R.drawable.ico_mail),
                PrimaryDrawerItem().withIdentifier(104)
                    .withIconTintingEnabled(true)
                    .withName("QR-код аудитории")
                    .withSelectable(false)
                    .withIcon(R.drawable.ico_qr_code),
                PrimaryDrawerItem().withIdentifier(105)
                    .withIconTintingEnabled(true)
                    .withName("Мое местоположение")
                    .withSelectable(false)
                    .withIcon(R.drawable.ico_map),
                PrimaryDrawerItem().withIdentifier(106)
                    .withIconTintingEnabled(true)
                    .withName("Настройки")
                    .withSelectable(false)
                    .withIcon(R.drawable.ico_settings)
            ).withOnDrawerItemClickListener(object : Drawer.OnDrawerItemClickListener {
                override fun onItemClick(
                    view: View?,
                    position: Int,
                    drawerItem: IDrawerItem<*>
                ): Boolean {
                    when (position) {
                        3 -> {
                            sendPostRequestAsync()
                        }
                        4 -> {
                            mainActivity.supportFragmentManager.beginTransaction()
                                .addToBackStack(null)
                                .replace(R.id.data_container, BaseFragment(R.layout.fragment_mail))
                                .commit()
                        }
                        5 -> {
                            val scanner = IntentIntegrator(mainActivity)
                            scanner.initiateScan()
                        }
                        6 -> {
                            val map = Intent(mainActivity, MapsActivity::class.java)
                            startActivity(mainActivity, map, Bundle.EMPTY)
                        }
                        7 -> {
                            mainActivity.supportFragmentManager.beginTransaction()
                                .addToBackStack(null)
                                .replace(
                                    R.id.data_container,
                                    BaseFragment(R.layout.fragment_settings)
                                ).commit()
                        }
                    }
                    return false
                }
            })
            .build()
    }

    private fun createHeader() {
        mHeader = AccountHeaderBuilder().withActivity(mainActivity)
            .withHeaderBackground(R.drawable.header)
            .addProfiles(
                ProfileDrawerItem().withName("Vanya Ivanov")
                    .withEmail("Group1")
            ).build()
    }

    fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        if (resultCode == Activity.RESULT_OK) {
            val result = IntentIntegrator.parseActivityResult(requestCode, resultCode, data)
            if (result != null) {
                if (result.getContents() == null) {
                    Toast.makeText(mainActivity, "Cancelled", Toast.LENGTH_LONG).show()
                } else {
                    Toast.makeText(mainActivity, "Scanned:" + result.contents, Toast.LENGTH_LONG).show()
                }
            } else {
                //super.onActivityResult(requestCode, resultCode, data)
            }
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