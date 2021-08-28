<template>
  <table class="table">
    <thead>
    <tr class="d-flex justify-content-between w-100">
      <th class="fs-4 fw-normal">Примеры</th>
      <th>
        <button class="btn btn-success" @click.prevent="$emit('update:examples', {type: 'add'})">Добавить</button>
      </th>
    </tr>
    </thead>
    <tbody class="container">
    <tr v-for="(example, example_idx) of examples" class="row">
      <td class="col-2 d-flex align-items-center justify-content-center">
        <span class="fs-5">
          {{ example.number }}
        </span>
      </td>
      <td class="col">
        <div class="d-flex align-items-center">
          <label class="w-50 fs-5">Входные данные</label>
          <textarea :value="example.inputText" @change="$emit('update:examples',{
            type: 'change',
            index: example_idx,
            value: {
              inputText: $event.target.value
            }
            })"
                    class="w-50 mb-2"></textarea>
        </div>
        <div class="d-flex align-items-center">
          <label class="w-50 fs-5">Выходные данные</label>
          <textarea :value="example.outputText" @change="$emit('update:examples', {
            type: 'change',
            index: example_idx,
            value:{
              outputText: $event.target.value
            }
            })"
                    class="w-50 mb-2">></textarea>
        </div>
      </td>
      <td class="col d-flex flex-column align-items-center justify-content-around">
        <div>

          <button class="btn btn-info me-2" style="width: 100px" @click.prevent="$emit('update:examples', {
          type: 'up-order',
          index: example_idx
          })">
            🠉
          </button>
          <button class="btn btn-info" style="width: 100px" @click.prevent="$emit('update:examples', {
            type: 'down-order',
            index: example_idx
          })">
            🠋
          </button>
        </div>
        <button class="btn btn-danger w-25" @click.prevent="$emit('update:examples', {
          type: 'delete',
          index: example_idx
        })">Удалить
        </button>

      </td>
    </tr>
    </tbody>
  </table>
</template>

<script>
import 'bootstrap-icons/font/bootstrap-icons.css';

export default {
  name: "ExamplesTableComponent",
  props: ['examples'],
  emits: ['update:examples'],
}
</script>

<style lang="scss" scoped>
tbody {
  tr {
    background-color: rgba(255, 255, 255, 0.2);
    margin-bottom: 0.625rem;
  }

  textarea {
    color: black;
    border-radius: 1rem;
  }
}
</style>