<template>
<!--eslint-disable -->
  <table class="table">
    <thead>
    <tr class="d-flex justify-content-between w-100">
      <th class="fs-4 fw-normal">Тесты</th>
      <th>
        <button class="btn btn-success" @click.prevent="$emit('update:tests', {type: 'add'})">Добавить</button>
      </th>
    </tr>
    </thead>
    <tbody class="container">
    <tr v-for="(test, test_idx) of tests" class="row">
      <td class="col-2 d-flex align-items-center justify-content-center">
        <span class="fs-5">
          {{ test.number }}
        </span>
      </td>
      <td class="col">
        <div class="d-flex align-items-center">
          <label class="w-50 fs-5">Входные данные</label>
          <textarea :value="test.input" @change="$emit('update:tests',{
            type: 'change',
            index: test_idx,
            value: {
              input: $event.target.value
            }
            })"
            class="w-50 mb-2"></textarea>
        </div>
        <div  class="d-flex align-items-center">
          <label class="w-50 fs-5">Ожидаемый ответ</label>
          <textarea :value="test.answer" @change="$emit('update:tests', {
            type: 'change',
            index: test_idx,
            value:{
              answer: $event.target.value
            }
            })"
            class="w-50 mb-2"></textarea>
        </div>
        <div>
          <label class="w-50 fs-5">Количество очков</label>
          <input type="number" :value="test.availablePoints" @change.prevent="$emit('update:tests', {
            type: 'change',
            index: test_idx,
            value:{
              availablePoints: $event.target.value
            }
          })"
          class="w-50 mb-2">
        </div>
      </td>
      <td class="col d-flex flex-column align-items-center justify-content-around">
        <div>
            <button class="btn btn-info me-2" style="width: 100px" @click.prevent="$emit('update:tests', {
            type: 'up-order',
            index: test_idx
          })">
                🠉
            </button>
            <button class="btn btn-info" style="width: 100px" @click.prevent="$emit('update:tests', {
            type: 'down-order',
            index: test_idx
          })">
                🠋
            </button>
        </div>
        <button class="btn btn-danger w-25" @click.prevent="$emit('update:tests', {
          type: 'delete',
          index: test_idx
        })">Удалить
        </button> 
      </td>
    </tr>
    </tbody>
  </table>

</template>

<script>
export default {
  name: "TestsTableComponent",
  props: ['tests'],
  emits: ['update:tests'],
}
</script>

<style lang="scss" scoped>
    tbody {
        tr {
            background-color: rgba(255, 255, 255, 0.2);
            margin-bottom: 10px;
        }

        textarea, input {
            color: black;
            border-radius: 16px;
        }
    }
</style>