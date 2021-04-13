package com.example.test2

import android.content.Intent
import android.os.Bundle
import android.os.StrictMode
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import com.example.test2.databinding.ActivityMainBinding
import com.example.test2.userinterface.fragments.BaseFragment
import com.example.test2.userinterface.objects.RegisterActivity
import com.example.test2.userinterface.objects.myDrawer
import com.squareup.moshi.Moshi
import com.squareup.moshi.kotlin.reflect.KotlinJsonAdapterFactory
import java.io.ByteArrayInputStream
import java.net.URL
import kotlin.random.Random

class MainActivity : AppCompatActivity() {
    private lateinit var mBinding: ActivityMainBinding
    private lateinit var mToolBar: Toolbar
    private lateinit var mmyDrawer: myDrawer

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        mBinding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(mBinding.root)
    }

    override fun onStart() {
        super.onStart()
        initFieleds()
        initFunc()
    }

    private fun initFunc(){
        if(false){
            setSupportActionBar(mToolBar)
            mmyDrawer.createDrawer()
            supportFragmentManager.beginTransaction()
                .replace(R.id.data_container, BaseFragment(R.layout.fragment_mail)).commit()
        }
        else{
            val register = Intent(this, RegisterActivity::class.java)
            startActivity(register)
        }

    }

    private fun initFieleds(){
        mToolBar = mBinding.mainToolBar
        mmyDrawer = myDrawer(this, mToolBar)
    }
}