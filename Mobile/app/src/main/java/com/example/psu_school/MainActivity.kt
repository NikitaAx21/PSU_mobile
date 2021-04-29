package com.example.psu_school

import android.os.Bundle
import android.os.StrictMode
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import com.example.psu_mobile.*
import com.example.psu_school.databinding.ActivityMainBinding
import com.example.psu_school.models.this_user
import com.example.psu_school.userinterface.fragments.EnterFragment
import com.example.psu_school.userinterface.fragments.WorkFragment
import com.example.psu_school.userinterface.objects.myDrawer
import com.example.psu_school.utilits.MAIN_ACTIVITY
import com.squareup.moshi.Moshi
import com.squareup.moshi.kotlin.reflect.KotlinJsonAdapterFactory
import utilits.replace_fragment
import utilits.show_toast
import java.io.ByteArrayInputStream
import java.net.URL
import kotlin.random.Random


class MainActivity : AppCompatActivity() {
    private lateinit var mBinding: ActivityMainBinding
    private lateinit var mToolBar: Toolbar
    lateinit var mmyDrawer: myDrawer
    lateinit var mUserInfo: this_user

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        mBinding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(mBinding.root)
        MAIN_ACTIVITY = this
        mUserInfo = this_user(
            Name = "Александр",
            Surname = "Пушкин",
            Midname = "Сергеевич",
            Group = "Лит-1"
        )
        initFieleds()
        initFunc()
    }

    private fun initFieleds() {
        mToolBar = mBinding.mainToolBar
        mmyDrawer = myDrawer(mToolBar, mUserInfo)
    }

    private fun initFunc() {
        setSupportActionBar(mToolBar)
        replace_fragment(EnterFragment(), false)
    }
}