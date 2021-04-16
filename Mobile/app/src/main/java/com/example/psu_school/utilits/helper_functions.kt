@file:Suppress("DEPRECATION")

package utilits

import android.content.Intent
import android.net.Uri
import android.provider.OpenableColumns
import android.view.View
import android.widget.EditText
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.fragment.app.Fragment
import com.example.psu_school.R
import com.example.psu_school.utilits.app_text_watcher
import com.google.android.material.floatingactionbutton.FloatingActionButton


//показывает сообщение во фрагментах
fun Fragment.show_toast(message: String){
    Toast.makeText(context, message, Toast.LENGTH_SHORT).show()
}

fun Fragment.button_visible(myEditText: EditText, myButton: FloatingActionButton){
    myEditText.addTextChangedListener(app_text_watcher {
        val string = myEditText.text.toString()
        if (string.isEmpty()) {
            myButton.visibility = View.VISIBLE
        } else {
            myButton.visibility = View.GONE
        }
    })
}

//функция для переключения активити
fun AppCompatActivity.replace_activity(activity: AppCompatActivity){
    val register = Intent(this, activity::class.java)
    startActivity(register)
}

fun AppCompatActivity.replace_fragment(fragment: Fragment){
    supportFragmentManager.beginTransaction()
        .addToBackStack(null)
        .replace(
            R.id.data_container,
            fragment
        ).commit()
}

fun Fragment.replace_fragment(fragment: Fragment){
    this.fragmentManager?.beginTransaction()
        ?.addToBackStack(null)
        ?.replace(R.id.data_container,
            fragment)?.commit()
}

fun Fragment.getFilenameFromUri(uri: Uri): String {
    var result = ""
    val cursor = context?.contentResolver?.query(uri, null, null, null, null)
    try {
        if (cursor != null && cursor.moveToFirst()) {
            result = cursor.getString(cursor.getColumnIndex(OpenableColumns.DISPLAY_NAME))
        }
    } catch (e: Exception) {
        show_toast(e.message.toString())
    } finally {
        cursor?.close()
        return result
    }
}