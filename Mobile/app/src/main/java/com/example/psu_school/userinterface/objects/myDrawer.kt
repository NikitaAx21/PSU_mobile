package com.example.psu_school.userinterface.objects

import android.annotation.SuppressLint
import android.graphics.drawable.Drawable
import android.net.Uri
import android.view.View
import android.widget.ImageView
import androidx.appcompat.widget.Toolbar
import androidx.core.content.res.TypedArrayUtils.getString
import androidx.drawerlayout.widget.DrawerLayout
import com.example.psu_school.MapsActivity
import com.example.psu_school.R
import com.example.psu_school.models.this_user
import com.example.psu_school.userinterface.fragments.*
import com.example.psu_school.utilits.*
import com.mikepenz.materialdrawer.*
import com.mikepenz.materialdrawer.model.PrimaryDrawerItem
import com.mikepenz.materialdrawer.model.ProfileDrawerItem
import com.mikepenz.materialdrawer.model.interfaces.IDrawerItem
import com.mikepenz.materialdrawer.util.AbstractDrawerImageLoader
import com.mikepenz.materialdrawer.util.DrawerImageLoader
import utilits.hide_key_board
import utilits.replace_activity
import utilits.replace_fragment
import utilits.set_image

class myDrawer(val toolbar: Toolbar, val mUserInfo: this_user) {
    private lateinit var mDrawer: Drawer
    private lateinit var mHeader: AccountHeader
    private lateinit var mDrawerLayout: DrawerLayout
    private lateinit var mCurrentProfile: ProfileDrawerItem

    fun createDrawer() {
        imageLoader()
        createHeader()
        createSubDrawer()
        mDrawerLayout = mDrawer.drawerLayout
    }

    //отключаем меню
    @SuppressLint("RestrictedApi")
    fun disableDrawer() {
        mDrawer.actionBarDrawerToggle?.isDrawerIndicatorEnabled = false
        MAIN_ACTIVITY.supportActionBar?.setDisplayHomeAsUpEnabled(true)
        mDrawerLayout.setDrawerLockMode(DrawerLayout.LOCK_MODE_LOCKED_CLOSED)//режим блокировки
        toolbar.setNavigationOnClickListener {
            MAIN_ACTIVITY.supportFragmentManager.popBackStack()
        }
    }

    //включаем меню
    @SuppressLint("RestrictedApi")
    fun enableDrawer() {
        MAIN_ACTIVITY.supportActionBar?.setDisplayHomeAsUpEnabled(false)
        mDrawer.actionBarDrawerToggle?.isDrawerIndicatorEnabled = true
        mDrawerLayout.setDrawerLockMode(DrawerLayout.LOCK_MODE_UNLOCKED)
        toolbar.setNavigationOnClickListener {
            mDrawer.openDrawer()
        }
    }

    //главная функция тулбара
    private fun createSubDrawer() {
        mDrawer = DrawerBuilder()
            .withActivity(MAIN_ACTIVITY)
            .withToolbar(toolbar)
            .withActionBarDrawerToggle(true)
            .withSelectedItem(-1)
            .withAccountHeader(mHeader)
            .addDrawerItems(
                PrimaryDrawerItem().withIdentifier(DRAWER_IND_WORK.toLong())
                    .withIconTintingEnabled(true)
                    .withName(MAIN_ACTIVITY.resources.getString(R.string.drawer_home_work_name))
                    .withSelectable(false)
                    .withIcon(R.drawable.ico_homework),
                PrimaryDrawerItem().withIdentifier(DRAWER_IND_MAIL.toLong())
                    .withIconTintingEnabled(true)
                    .withName(MAIN_ACTIVITY.resources.getString(R.string.drawer_mail_name))
                    .withSelectable(false)
                    .withIcon(R.drawable.ico_mail),
                PrimaryDrawerItem().withIdentifier(DRAWER_IND_CODE.toLong())
                    .withIconTintingEnabled(true)
                    .withName(MAIN_ACTIVITY.resources.getString(R.string.drawer_qr_code_name))
                    .withSelectable(false)
                    .withIcon(R.drawable.ico_qr_code),
                PrimaryDrawerItem().withIdentifier(DRAWER_IND_MAP.toLong())
                    .withIconTintingEnabled(true)
                    .withName(MAIN_ACTIVITY.resources.getString(R.string.drawer_map_name))
                    .withSelectable(false)
                    .withIcon(R.drawable.ico_map),
                PrimaryDrawerItem().withIdentifier(DRAWER_IND_SETTINGS.toLong())
                    .withIconTintingEnabled(true)
                    .withName(MAIN_ACTIVITY.resources.getString(R.string.drawer_settings_name))
                    .withSelectable(false)
                    .withIcon(R.drawable.ico_settings)
            ).withOnDrawerItemClickListener(object : Drawer.OnDrawerItemClickListener {
                override fun onItemClick(
                    view: View?,
                    position: Int,
                    drawerItem: IDrawerItem<*>
                ): Boolean {
                    when (position) {
                        DRAWER_POS_WORK -> {
                            MAIN_ACTIVITY.replace_fragment(WorkFragment())
                        }
                        DRAWER_POS_MAIL -> {
                            MAIN_ACTIVITY.replace_fragment(MailFragment())
                        }
                        DRAWER_POS_CODE -> {
                            MAIN_ACTIVITY.replace_fragment(PhotoFragment())
                        }
                        DRAWER_POS_MAP -> {
                            MAIN_ACTIVITY.replace_activity(MapsActivity())
                        }
                        DRAWER_POS_SETTINGS -> {
                            MAIN_ACTIVITY.replace_fragment(SettingsFragment())
                        }
                    }
                    return false
                }
            })
            .build()
    }

    private fun createHeader() {
        mCurrentProfile = ProfileDrawerItem()
            .withName(mUserInfo.Name + " " + mUserInfo.Surname)
            .withEmail(mUserInfo.Group)
            .withIcon(R.drawable.ico_student_4)
        mHeader = AccountHeaderBuilder()
            .withActivity(MAIN_ACTIVITY)
            .withHeaderBackground(R.drawable.header)
            .addProfiles(
                mCurrentProfile
            ).build()
    }

    fun updateHeader() {
            mCurrentProfile
                .withName(mUserInfo.Name + " " + mUserInfo.Surname)
                .withEmail(mUserInfo.Group)
                .withIcon(mUserInfo.Url_file)

        mHeader.updateProfile(mCurrentProfile)
    }

    private fun imageLoader() {
        DrawerImageLoader.init(object : AbstractDrawerImageLoader() {
            override fun set(imageView: ImageView, uri: Uri, placeholder: Drawable) {
                imageView.set_image(uri.toString())
            }
        })
    }
}