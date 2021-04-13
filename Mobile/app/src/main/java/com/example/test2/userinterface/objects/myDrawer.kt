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
    //главная функция тулбара
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

                        }
                        4 -> {
                            mainActivity.supportFragmentManager.beginTransaction()
                                .addToBackStack(null)
                                .replace(
                                    R.id.data_container,
                                    BaseFragment(R.layout.fragment_mail))
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

}