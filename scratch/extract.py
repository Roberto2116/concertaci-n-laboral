import sqlite3

conn = sqlite3.connect(r'c:\Users\Thinkpad\Desktop\Practicas\Proyecto_GRRLN_expediente\db\PEMEXDB.db')
cursor = conn.cursor()
cursor.execute("SELECT sql FROM sqlite_master WHERE type='table'")
with open(r'c:\Users\Thinkpad\Desktop\Practicas\Proyecto_GRRLN_expediente\scratch\schema.sql', 'w', encoding='utf-8') as f:
    for row in cursor.fetchall():
        if row[0]:
            f.write(row[0] + ';\n\n')
print("Schema extracted")
conn.close()
