<template>
    <div>
        <div class="hero is-medium">
            <div class="hero-body">
                <div class="container">
                    <h1 class="title">Sandbank</h1>
                    <h2 class="subtitle">Experience banking paradise</h2>
                </div>
            </div>
        </div>
        <div class="tile is-ancestor">
            <div class="tile is-vertical is-4">
                <div class="tile">
                    <div class="tile is-parent is-vertical">
                        <article class="tile is-child box">
                            <p class="title">Home Loans</p>
                            <p class="subtitle">Get our great home loan rate.</p>
                        </article>
                        <article class="tile is-child box">
                            <p class="title">Accounts</p>
                            <p class="subtitle">Keep on moving with an everyday account.</p>
                        </article>
                        <article class="tile is-child box">
                            <p class="title">Find Out More</p>
                            <p class="subtitle">We support the things that are important to you.</p>
                        </article>
                    </div>
                </div>
            </div>
            <div class="tile is-parent">
                <article class="tile is-child notification is-info">
                    <div class="content">
                        <p class="title">Home Loan Calculator</p>
                        <p class="subtitle">Get an idea of what your repayments might be.</p>
                        <div class="content">
                            <b-field label="Loan Amount" custom-class="has-text-white">
                                <b-input
                                    numeric
                                    v-model="loanAmount"
                                    icon-pack="fas"
                                    icon="dollar-sign"></b-input>
                            </b-field>
                            <b-field label="Interest Rate" custom-class="has-text-white">
                                <b-input 
                                    numeric 
                                    v-model="interestRate"
                                    icon-pack="fas"
                                    icon="percent"></b-input>
                            </b-field>
                            <b-field label="Loan Length" custom-class="has-text-white">
                                <b-select v-model="loanLength">
                                    <option :value="30">
                                        30 Years
                                    </option>
                                    <option :value="25">
                                        25 Years
                                    </option>
                                    <option :value="20">
                                        20 Years
                                    </option>
                                    <option :value="15">
                                        15 Years
                                    </option>
                                    <option :value="10">
                                        10 Years
                                    </option>
                                    <option :value="5">
                                        5 Years
                                    </option>
                                </b-select>
                            </b-field>
                        </div>
                    </div>
                </article>
            </div>
            <div class="tile is-parent">
                <article class="tile is-child box">
                    <p class="title">Talk To An Expert</p>
                    <p class="subtitle">Our mobile managers can come to you.</p>
                    <figure class="image is-4by3">
                        <img src="https://images.unsplash.com/photo-1488998527040-85054a85150e?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=640&q=80">
                    </figure>
                </article>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';

@Component
export default class HomePage extends Vue {

    private loanAmount: number = 400000;
    private interestRate: number = 4.89;
    private loanLength: number = 30;

}
</script>

<style>

</style>
