package com.example.psu_school.userinterface.objects

import android.annotation.SuppressLint
import android.app.Activity
import android.content.Intent
import android.view.View
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import androidx.drawerlayout.widget.DrawerLayout
import com.example.psu_mobile.UserInfo
import com.example.psu_school.MapsActivity
import com.example.psu_school.R
import com.example.psu_school.userinterface.fragments.*
import com.google.zxing.integration.android.IntentIntegrator
import com.mikepenz.materialdrawer.*
import com.mikepenz.materialdrawer.model.PrimaryDrawerItem
import com.mikepenz.materialdrawer.model.ProfileDrawerItem
import com.mikepenz.materialdrawer.model.interfaces.IDrawerItem
import kotlinx.android.synthetic.main.fragment_photo.*
import utilits.replace_activity
import utilits.replace_fragment

class myDrawer(val mainActivity: AppCompatActivity, val toolbar: Toolbar, val mUserInfo: UserInfo){
    private lateinit var mDrawer: Drawer
    private lateinit var mHeader: AccountHeader
    private lateinit var mDrawerLayout: DrawerLayout

    public fun createDrawer(){
        createHeader()
        createSubDrawer()
        mDrawerLayout = mDrawer.drawerLayout
    }

    //отключаем меню
    @SuppressLint("RestrictedApi")
    fun disableDrawer(){
        mDrawer.actionBarDrawerToggle?.isDrawerIndicatorEnabled = false
        mainActivity.supportActionBar?.setDisplayHomeAsUpEnabled(true)
        mDrawerLayout.setDrawerLockMode(DrawerLayout.LOCK_MODE_LOCKED_CLOSED)//режим блокировки
        toolbar.setNavigationOnClickListener {
            mainActivity.supportFragmentManager.popBackStack()
        }
    }

    //включаем меню
    @SuppressLint("RestrictedApi")
    fun enableDrawer(){
        mainActivity.supportActionBar?.setDisplayHomeAsUpEnabled(false)
        mDrawer.actionBarDrawerToggle?.isDrawerIndicatorEnabled = true
        mDrawerLayout.setDrawerLockMode(DrawerLayout.LOCK_MODE_UNLOCKED)
        toolbar.setNavigationOnClickListener {
            mDrawer.openDrawer()
        }
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
                        1 -> {
                            mainActivity.replace_fragment(NewsFragment())
                        }
                        2 -> {
                            mainActivity.replace_fragment(TimeTableFragment())
                        }
                        3 -> {
                            mainActivity.replace_fragment(HomeWorkFragment())
                        }
                        4 -> {
                            mainActivity.replace_fragment(MailFragment())
                        }
                        5 -> {
                            mainActivity.replace_fragment(PhotoFragment())
                        }
                        6 -> {
                            mainActivity.replace_activity(MapsActivity())
                        }
                        7 -> {
                            mainActivity.replace_fragment(SettingsFragment())
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
                ProfileDrawerItem().withName(mUserInfo.Name + " " + mUserInfo.Surname)
                    .withEmail(mUserInfo.Group)
            ).build()
    }
}