import re
import pprint
import csv, sqlite3


def _price(price:str):
    return int(re.sub(r'[^\d]', '', price))

def create_db_from_datacsv():
    con = sqlite3.connect('app.db')
    cur = con.cursor()
    cols = "(type, name, ingredient1, ingredient2, ingredient3, ingredient4, base_value, effect, unlock_from, festivals)"
    cur.execute(f"CREATE TABLE recipes (id INTEGER PRIMARY KEY, type, name, ingredient1, ingredient2, ingredient3, ingredient4, base_value INTEGER, effect, unlock_from, festivals)")
    con.commit()

    with open('data.csv', 'r', encoding='utf-8') as f:
        dr = csv.DictReader(f)
        print (dr)

        cur_type = ''
        for idx, i in enumerate(dr):
            if i['Recipe Image'] != '':
                print(i)
                cur_type = i['Recipe Image']
                pprint.pprint(i)
            else:
                entry_values = (cur_type, i['Recipe Name'], i['Ingredient 1'], i['Ingredient 2'], i['Ingredient 3'], i['Ingredient 4'], _price(i['Base Value']), i['Effect'], i['Where to Unlock'], i['Used for Festivals'])
                insert_sql = f"INSERT INTO recipes{cols} VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
                cur.execute(insert_sql, entry_values)
                con.commit()
    cur.close()


def print_db():
    try:
        with sqlite3.connect('app.db') as conn:
            # create a cursor
            cursor = conn.cursor()

            # execute statements
            cursor.execute("SELECT * FROM recipes")
            rows = cursor.fetchall()
            pprint.pprint(rows)
            print(len(rows))

    except sqlite3.OperationalError as e:
        pprint.pprint(e)

def clean_ingredients_data():
    try:
        with sqlite3.connect('app.db') as conn:
            # create a cursor
            cursor = conn.cursor()
            cursor.execute("CREATE TABLE items (id INTEGER PRIMARY KEY, name type UNIQUE NOT NULL)")
            conn.commit()

            # execute statements
            cursor.execute("SELECT DISTINCT name FROM (" \
                "SELECT DISTINCT ingredient1 as name FROM recipes UNION ALL " \
                "SELECT DISTINCT ingredient2 as name FROM recipes WHERE ingredient2 IS NOT NULL UNION ALL " \
                "SELECT DISTINCT ingredient3 as name FROM recipes WHERE ingredient3 IS NOT NULL UNION ALL " \
                "SELECT DISTINCT ingredient4 as name FROM recipes WHERE ingredient4 IS NOT NULL " \
                ") as cname " \
                "ORDER BY name"
            )
            rows = cursor.fetchall()
            pprint.pprint(rows)
            print(len(rows))
            print('Start creating item entry')
            all_items = set()
            for row in rows:
                # print(row)
                for item in row[0].strip().split(', '):
                    item = str(item).strip()
                    if (len(item) == 0):
                        continue
                    any_tag = ' (any)'
                    if str(item).endswith(any_tag):
                        item_name = str(item).replace(any_tag, '')
                        print(f"Create record for item types: {item}, name: {item_name}")
                        if item_name == 'Honey':
                            all_items.add("Honey")
                            all_items.add("Invigorating Honey")
                            all_items.add("Mellow Honey")
                            all_items.add("Royal Honey")
                        elif item_name in ['Mushroom', 'Mushrom']:
                            all_items.add("Shimeji Mushroom")
                            all_items.add("Common Mushroom")
                            all_items.add("Porcini Mushroom")                            
                            all_items.add("Morel Mushroom")
                            all_items.add("Monarch Mushroom")
                            all_items.add("Matsutake Mushroom")
                        elif item_name in ['Cheese', 'Butter', 'Mayonnaise']:
                            all_items.add(f"{item_name}")
                            all_items.add(f"Herb {item_name}")
                            all_items.add(f"{item_name}+")
                        else:
                            print(f"TODO: CREATE ENTRIES FOR THIS: {item}")
                    else:
                        all_items.add(item)

            print("start saving to db...")
            all_items = sorted(list(all_items))
            for item in all_items:
                print(item)
                insert_sql = f"INSERT INTO items(name) VALUES(?)"
                cursor.execute(insert_sql, [str(item)])
                conn.commit()


    except sqlite3.OperationalError as e:
        pprint.pprint(f"SQLite3 OperationalError: {e}")


def map_recipes_to_items():
    try:
        with sqlite3.connect('app.db') as conn:
            # create a cursor
            cursor = conn.cursor()
            cursor.execute("CREATE TABLE recipe_item_map (" \
                "recipe_id INTEGER NOT NULL, " \
                "item_id INTEGER NOT NULL, " \
                "is_in1 INTEGER DEFAULT 0, " \
                "is_in2 INTEGER DEFAULT 0, " \
                "is_in3 INTEGER DEFAULT 0, " \
                "is_in4 INTEGER DEFAULT 0, " \
                "PRIMARY KEY(recipe_id, item_id)" \
                ");"
            )
            conn.commit()

            for ingredient_slot in range(1, 5):
                # execute statements
                cursor.execute(f"SELECT id, ingredient{ingredient_slot} FROM recipes WHERE ingredient{ingredient_slot} IS NOT NULL")
                rows = cursor.fetchall()
                print(f"Creating records for ingredient{ingredient_slot}....")

                for recipe_id, row in rows:
                    for item in row.split(', '):
                        item = str(item).strip()
                        any_tag = ' (any)'
                        if str(item).endswith(any_tag):
                            item_name = str(item).replace(any_tag, '')
                            # print(f"Create record for item types: {item}, name: {item_name}")
                            if item_name == 'Honey':
                                _insert_or_update_map(cursor, ingredient_slot, recipe_id, item="Honey")

                                _insert_or_update_map(cursor, ingredient_slot, recipe_id, "Invigorating Honey")

                                _insert_or_update_map(cursor, ingredient_slot, recipe_id, "Mellow Honey")

                                _insert_or_update_map(cursor, ingredient_slot, recipe_id, item="Royal Honey")

                            elif item_name in ['Mushroom', 'Mushrom']:
                                _insert_or_update_map(cursor, ingredient_slot, recipe_id, "Shimeji Mushroom")

                                _insert_or_update_map(cursor, ingredient_slot, recipe_id, "Common Mushroom")

                                _insert_or_update_map(cursor, ingredient_slot, recipe_id, "Porcini Mushroom")

                                _insert_or_update_map(cursor, ingredient_slot, recipe_id, "Morel Mushroom")

                                _insert_or_update_map(cursor, ingredient_slot, recipe_id, "Monarch Mushroom")

                                _insert_or_update_map(cursor, ingredient_slot, recipe_id, "Matsutake Mushroom")

                            elif item_name in ['Cheese', 'Butter', 'Mayonnaise']:
                                _insert_or_update_map(cursor, ingredient_slot, recipe_id, item_name)

                                _insert_or_update_map(cursor, ingredient_slot, recipe_id, f"Herb {item_name}")

                                _insert_or_update_map(cursor, ingredient_slot, recipe_id, f"{item_name}+")

                            else:
                                print(f"TODO: CREATE ENTRIES FOR THIS: {item}")
                        else:
                            # all_items.add(item)
                            _insert_or_update_map(cursor, ingredient_slot, recipe_id, item)
                            
                            conn.commit()

    except sqlite3.OperationalError as e:
        pprint.pprint(e)

def _insert_or_update_map(cursor, ingredient_slot, recipe_id, item):
    print(f"Inserting record: {recipe_id}, {item}...")
    try:
        cursor.execute(f"INSERT INTO recipe_item_map(recipe_id, item_id, is_in{ingredient_slot}) SELECT {recipe_id}, id, 1 FROM items WHERE name = \"{item}\"")
    except sqlite3.IntegrityError as e:
        pprint.pprint(e)
        if ingredient_slot > 1:
                                    # PK already exists: update is_in<2/3/4> value to 1
            item_id = _get_item_id_by_name(item, cursor)
            cursor.execute(f"UPDATE recipe_item_map SET is_in{ingredient_slot} = 1 WHERE recipe_id = {recipe_id} AND item_id = {item_id}")
        else:
            raise Exception('FIX THIS! THIS SHOULD NOT BE!!!')


def _get_item_id_by_name(name, cursor):
    cursor.execute(f"SELECT id FROM items WHERE name = \"{name}\"")
    id = cursor.fetchone()[0]
    return id

def create_filter_categories():
    # Vegetables, Fruits, Products, Forageable, Fish, Others
    try:
        with sqlite3.connect('app.db') as conn:
            # create a cursor
            cursor = conn.cursor()

            # execute statements
            cursor.execute("SELECT * FROM items")
            rows = cursor.fetchall()

            with open('category.csv', 'w', newline='') as file:
                writer = csv.writer(file)
                writer.writerow(["id", "name", "category"])
                for row in rows:
                    id, name, category = row
                    writer.writerow([id, name, category])

    except sqlite3.OperationalError as e:
        pprint.pprint(e)
    pass


if __name__ == "__main__":
    # create_db_from_datacsv()
    # print_db()
    # clean_ingredients_data()
    # map_recipes_to_items()
    # create_filter_categories()

    # todo: create script to apply categries to items table if needed
    pass